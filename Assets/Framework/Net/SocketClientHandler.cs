// ========================================================
// 描 述：SocketClientHandler.cs 
// 作 者：郑贤春 
// 时 间：2019/06/25 23:55:22 
// 版 本：2018.3.12f1 
// ========================================================
using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;
using Framework.Utility;

namespace Framework.Net
{
    class SocketClientHandler : IDisposable
    {
        Socket mClient;
        ISocketClient mSocketClient;
        byte[] buffer = new byte[1024];
        int tempLen;
        byte[] tempBuffer;

        public SocketClientHandler() : this("47.110.72.141", 10007)
        {
            
        }

        public SocketClientHandler(string ip, int port)
        {
            this.mClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            try
            {
                this.mClient.Connect(iPEndPoint);
                Debug.Log("Server Connected!");
                this.mClient.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), mClient);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        public SocketClientHandler(ISocketClient client) : this(client,NetUtility.GetIP(), 2000)
        {

        }

        public SocketClientHandler(ISocketClient client,string ip,int port)
        {
            this.mSocketClient = client;
            this.mClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            try
            {
                this.mClient.Connect(iPEndPoint);
                Debug.Log("Server Connected!");
                this.mClient.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), mClient);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        public void SetSocketClient(ISocketClient client)
        {
            this.mSocketClient = client;
        }

        public void SendMessage(byte[] bytes)
        {
            byte[] finalBytes = EncodeBytes(bytes);
            this.mClient.Send(finalBytes);
        }

        public void Dispose()
        {

        }

        void ReceiveMessage(IAsyncResult ar)
        {
            try
            {
                var client = ar.AsyncState as Socket;
                var length = client.EndReceive(ar);
                DecodeBytes(buffer, length);
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

        void DecodeBytes(byte[] buffer,int length)
        {
            //读取出来消息内容
            int index = 0;
            if (tempBuffer != null)
            {
                int len = tempBuffer.Length - tempLen;
                if (index + len <= length)
                {
                    Array.Copy(buffer, index, tempBuffer, tempLen, len);
                    index += tempBuffer.Length - tempLen;
                    if (mSocketClient != null) mSocketClient.Receive(tempBuffer);
                    tempBuffer = null;
                }
                else
                {
                    len = length;
                    Array.Copy(buffer, index, tempBuffer, tempLen, len);
                    index += len;
                    tempLen += len;
                }
            }
            while (index < length)
            {
                byte[] lenbytes = new byte[4];
                Array.Copy(buffer, index, lenbytes, 0, 4);
                int len = System.BitConverter.ToInt32(lenbytes, 0);
                index += 4;
                //显示消息
                byte[] bytes = new byte[len];
                if (index + len <= length)
                {
                    Array.Copy(buffer, index, bytes, 0, len);
                }
                else
                {
                    tempLen = length - index;
                    Array.Copy(buffer, index, bytes, 0, tempLen);
                    tempBuffer = bytes;
                    break;
                }
                if (mSocketClient != null) mSocketClient.Receive(bytes);
                index += len;
            }
        }

        byte[] EncodeBytes(byte[] bytes)
        {
            //int length = bytes.Length;
            //byte[] lenBytes = System.BitConverter.GetBytes(length);
            //byte[] finalBytes = new byte[bytes.Length + lenBytes.Length];
            //Array.Copy(lenBytes, 0, finalBytes, 0, lenBytes.Length);
            //Array.Copy(lenBytes, 0, finalBytes, 0, lenBytes.Length);
            //lenBytes.CopyTo(finalBytes, 0);
            //bytes.CopyTo(finalBytes, lenBytes.Length);
            //return finalBytes;

            byte[] idBytes = System.BitConverter.GetBytes((short)111);
            int length = bytes.Length;
            byte[] lenBytes = System.BitConverter.GetBytes((short)length);
            byte[] finalBytes = new byte[idBytes.Length + lenBytes.Length + bytes.Length];
            Array.Copy(lenBytes, 0, finalBytes, 0, lenBytes.Length);
            Array.Copy(idBytes, 0, finalBytes, lenBytes.Length, idBytes.Length);
            Array.Copy(bytes, 0, finalBytes, idBytes.Length + lenBytes.Length, bytes.Length);
            return finalBytes;
        }

        public void Shutdown()
        {
            this.mClient.Shutdown(SocketShutdown.Both);
            this.mClient.Close();
        }
    }
}



