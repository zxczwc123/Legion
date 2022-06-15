// ========================================================
// 描 述：SocketServerHandler.cs 
// 作 者：郑贤春 
// 时 间：2019/06/25 23:55:22 
// 版 本：2018.3.12f1 
// ========================================================
using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System;
using Framework.Utility;

namespace Framework.Net
{
    public class SocketServerHandler : IDisposable
    {
        Socket mServer;

        ISocketServer mSocketServer;

        List<Socket> mClients;

        byte[] buffer = new byte[1024];
        byte[] tempBuffer;
        int tempLen;

        public SocketServerHandler (): this(NetUtility.GetIP(), 2000)
        {
            
        }

        public SocketServerHandler(string ip,int port) : this(null, NetUtility.GetIP(), 2000)
        {

        }

        public SocketServerHandler(ISocketServer server) : this(server,NetUtility.GetIP(), 2000)
        {

        }

        public SocketServerHandler(ISocketServer server, string ip, int port)
        {
            mSocketServer = server;
            this.mClients = new List<Socket>();
            this.mServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            this.mServer.Bind(iPEndPoint);
            this.mServer.Listen(10);
            Debug.Log(string.Format("Server {0} Start", this.mServer.LocalEndPoint.ToString()));
            this.mServer.BeginAccept(new AsyncCallback(ClientListen), mServer);
        }

        private void ClientListen(IAsyncResult ar)
        {
            try
            {
                Socket server = ar.AsyncState as Socket;
                Socket client = server.EndAccept(ar);
                Debug.Log(string.Format("Client {0} connect!", client.LocalEndPoint.ToString()));
                if (!this.mClients.Contains(client)) this.mClients.Add(client);
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);
                server.BeginAccept(new AsyncCallback(ClientListen), server);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void ReceiveMessage(IAsyncResult ar)
        {
            try
            {
                var client = ar.AsyncState as Socket;
                var length = client.EndReceive(ar);
                //读取出来消息内容
                int index = 0;
                if (tempBuffer != null)
                {
                    int len = tempBuffer.Length - tempLen;
                    if (index + len <= length)
                    {
                        Array.Copy(buffer, index, tempBuffer, tempLen, len);
                        index += tempBuffer.Length - tempLen;
                        if (mSocketServer != null) mSocketServer.Receive(tempBuffer);
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
                    if(index + len <= length)
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
                    if (mSocketServer != null) mSocketServer.Receive(bytes);
                    index += len;
                }
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

        public void SendMessage(Socket client, byte[] buf)
        {
            byte[] bytes = EncodeBytes(buf);
            client.Send(bytes);
        }

        public void SendMessageAll(byte[] buf)
        {
            foreach (Socket client in this.mClients)
            {
                SendMessage(client, buf);
            }
        }

        byte[] EncodeBytes(byte[] bytes)
        {
            int length = bytes.Length;
            byte[] lenBytes = System.BitConverter.GetBytes(length);
            byte[] finalBytes = new byte[bytes.Length + lenBytes.Length];
            Array.Copy(lenBytes, 0, finalBytes, 0, lenBytes.Length);
            Array.Copy(lenBytes, 0, finalBytes, 0, lenBytes.Length);
            lenBytes.CopyTo(finalBytes, 0);
            bytes.CopyTo(finalBytes, lenBytes.Length);
            return finalBytes;
        }

        public void Dispose()
        {
            
        }

        public void Shutdown()
        {
            this.mServer.Shutdown(SocketShutdown.Both);
            this.mServer.Close();
        }
    }

}
