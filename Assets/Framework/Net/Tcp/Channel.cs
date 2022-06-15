// ========================================================
// 描 述：Channal.cs 不同服务器独立连接
// 创 建：高辉 
// 时 间：2022/01/13 09:41:58 
// 版 本：2020.3.18f1c1 
// ========================================================

using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Framework.SocketNet {
    
    public class Channel {
        public bool IsDisposed
        {
            get {
                return this.RemoteAddress == null;
            }
        }

        public bool IsDisConnect {
            get {
                return this._client == null || !this._client.Connected;
            }
        }
        
        public IPEndPoint RemoteAddress { get; private set; }
        
        private Action _onConnectCallback;
        
        private Socket _client;

        private bool isConnected;
        
        private bool isSending;
        
        private Action<int,IPEndPoint> _aError;

        private Action<byte[],IPEndPoint> _aRead;

        private readonly Circlebytes _recvBuffers = new Circlebytes();

        private readonly Circlebytes _sendBuffers = new Circlebytes();

        private SocketAsyncEventArgs innArgs = new SocketAsyncEventArgs();

        private SocketAsyncEventArgs outArgs = new SocketAsyncEventArgs();
        
        public void SetAction(Action<int,IPEndPoint> aError, Action<byte[],IPEndPoint> aRead) {
            this._aError = aError;
            this._aRead = aRead;
        }

        public Channel(IPEndPoint ip, Action callback) {
            this._client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._client.NoDelay = true;
            this._onConnectCallback = callback;
            this.RemoteAddress = ip;
            
            this.isConnected = false;
            this.innArgs.Completed += OnCompete;
            this.outArgs.Completed += OnCompete;
            
            ThreadSynchronizationContext.Instance.PostNext(this.ConnectAsync);
        }
        
        private void OnCompete(object sender, SocketAsyncEventArgs e) {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    ThreadSynchronizationContext.Instance.Post(()=>OnConnectComplete(e));
                    break;
                case SocketAsyncOperation.Receive:
                    ThreadSynchronizationContext.Instance.Post(()=>OnRecvComplete(e));
                    break;
                case SocketAsyncOperation.Send:
                    ThreadSynchronizationContext.Instance.Post(()=>OnSendComplete(e));
                    break;
                case SocketAsyncOperation.Disconnect:
                    ThreadSynchronizationContext.Instance.Post(()=>OnDisconnectComplete(e));
                    break;
                default:
                    throw new Exception($"socket error: {e.LastOperation}");
            }
        }
        
        
        private void ConnectAsync() {
            this.outArgs.RemoteEndPoint = this.RemoteAddress;
            if (this._client.ConnectAsync(this.outArgs)) return;
            this.OnConnectComplete(this.outArgs);
        }
        
        private void OnConnectComplete(object o) {
            if (this._client == null) return;
            SocketAsyncEventArgs e = o as SocketAsyncEventArgs;
            if (e.SocketError != SocketError.Success) {
                ThreadSynchronizationContext.Instance.PostNext(() => this._aError?.Invoke((int)e.SocketError,this.RemoteAddress));
                return;
            }
            
            e.RemoteEndPoint = null;
            ThreadSynchronizationContext.Instance.PostNext(() => this._aError?.Invoke((int)ErrorCode.ConnectSuccess,this.RemoteAddress));
            this.isConnected = true;
            this.StartRecv();
            this.StartSend();
            
            ThreadSynchronizationContext.Instance.PostNext(() => {
                this._onConnectCallback?.Invoke();
                this._onConnectCallback = null;
            });
        }
        
        private void OnDisconnectComplete(object o)
        {
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
            if (e.SocketError == SocketError.Success)
                ThreadSynchronizationContext.Instance.PostNext(() => this._aError?.Invoke((int)ErrorCode.Disconnect,this.RemoteAddress));
        }
        
        private void OnRecvComplete(object o)
        {
            this.HandleRecv(o);
            if (this._client == null) return;
            this.StartRecv();
        }

        private void OnSendComplete(object o) {
            this.HandleSend(o);
            if (this._client == null) return;
            this.StartSend();
        }
        
        private void StartRecv() {
            while (true) {
                if (this._client == null) return;
                try {
                    int size = this._recvBuffers.MaxCapacity - this._recvBuffers.LastIndex;
                    this.innArgs.SetBuffer(this._recvBuffers._lastBytes,this._recvBuffers.LastIndex,size);
                } catch {
                    ThreadSynchronizationContext.Instance.PostNext(() => this._aError?.Invoke((int)ErrorCode.RecvError,this.RemoteAddress));
                    return;
                }
                if(this._client.ReceiveAsync(this.innArgs)) return;
                this.HandleRecv(this.innArgs);
            }
        }

        private void StartSend() {
            if(!this.isConnected)
            {
                return;
            }
            while (true) {
                if (this._client == null) return;
                if (this._sendBuffers.Length == 0) {
                    this.isSending = false;
                    return;
                }
                try {
                    this.isSending = true;
                    int size = this._sendBuffers.MaxCapacity - this._sendBuffers.FirstIndex;
                    if (size > this._sendBuffers.Length) size = this._sendBuffers.Length;
                    this.outArgs.SetBuffer(this._sendBuffers.FirstBytes,this._sendBuffers.FirstIndex,size);
                    if (this._client.SendAsync(this.outArgs)) return;
                    HandleSend(this.outArgs);
                } catch (Exception e) {
                    throw new Exception($"socket set buffer error: {this._sendBuffers.FirstBytes.Length}, {this._sendBuffers.FirstIndex}", e);
                }
            }
        }

        private void HandleRecv(object o) {
            if (this._client == null) return;
            SocketAsyncEventArgs e = o as SocketAsyncEventArgs;
            if (e.SocketError != SocketError.Success) {
                ThreadSynchronizationContext.Instance.PostNext(() => this._aError?.Invoke((int)e.SocketError,this.RemoteAddress));
                return;
            }

            if (e.BytesTransferred == 0) {
                ThreadSynchronizationContext.Instance.PostNext(() => this._aError?.Invoke((int)ErrorCode.PeerDisconnect,this.RemoteAddress));
                return;
            }

            this._recvBuffers.LastIndex += e.BytesTransferred;
            if (this._recvBuffers.LastIndex == this._recvBuffers.MaxCapacity) {
                this._recvBuffers.AddQueue();
                this._recvBuffers.LastIndex = 0;
            }

            // 有消息就push倒
            while (true) {
                if (this._recvBuffers.Length == 0) return;
                try {
                    var bytes = new byte[this._recvBuffers.Length];
                    this._recvBuffers.Read(bytes,0,this._recvBuffers.Length);
                    this._aRead?.Invoke(bytes,this.RemoteAddress);
                } catch (Exception exception) {
                    Console.WriteLine(exception);
                    throw;
                }
            }
        }

        private void HandleSend(object o) {
            if (this._client == null) return;
            SocketAsyncEventArgs e = o as SocketAsyncEventArgs;
            if (e.SocketError != SocketError.Success) {
                ThreadSynchronizationContext.Instance.PostNext(() => this._aError?.Invoke((int)e.SocketError,this.RemoteAddress));
                return;
            }

            if (e.BytesTransferred == 0) {
                ThreadSynchronizationContext.Instance.PostNext(() => this._aError?.Invoke((int)ErrorCode.PeerDisconnect,this.RemoteAddress));
                return;
            }

            this._sendBuffers.FirstIndex += e.BytesTransferred;
            if (this._sendBuffers.FirstIndex == this._sendBuffers.MaxCapacity) {
                this._sendBuffers.RemoveFirst();
                this._sendBuffers.FirstIndex = 0;
            }
        }

        public void Send(byte[] bytes) {
            if (this._client == null || this.IsDisposed) return;
            this._sendBuffers.Write(bytes,0,bytes.Length);
            if (!this.isSending && (this._client != null ? this._client.Connected : false)) this.StartSend();
        }
        
        public void Dispose() {
            if (this.IsDisposed) return;
            Debug.Log($"{this.RemoteAddress} is Disposed!!");
            this._client?.Close();
            this.innArgs.Dispose();
            this.outArgs.Dispose();
            this.innArgs = null;
            this.outArgs = null;
            this._client = null;
            this._aError = null;
            this._aRead = null;
            this.RemoteAddress = null;
        }
    }
}
