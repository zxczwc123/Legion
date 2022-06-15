// ========================================================
// 描 述：AsyncSocketServer 17-29-13-235.cs 
// 作 者：郑贤春 
// 时 间：2019/06/15 17:31:59 
// 版 本：2018.3.12f1 
// ========================================================
using UnityEngine;
using System.Collections;
using System;

namespace Framework.Net
{
    public class AsyncSocketServer<T> : ISocketServer ,IDisposable
    {
        private SocketServerHandler serverHandler;
        /// <summary>
        /// 数据编码器
        /// </summary>
        private AsyncSocketDataCoder<T> m_dataCoder;

        public AsyncSocketServer()
        {
            if (serverHandler == null) serverHandler = new SocketServerHandler(this);
        }

        public void PostAll(T obj)
        {
            byte[] bytes = Encode(obj);
            serverHandler.SendMessageAll(bytes);
        }

        /// <summary>
        /// 设置数据编码器
        /// </summary>
        /// <param name="dataCoder"></param>
        public void SetDataCoder(AsyncSocketDataCoder<T> dataCoder){
            m_dataCoder = dataCoder;
        }

        protected T Decode(byte[] bytes){
            return m_dataCoder.Decode(bytes);
        }

        protected byte[] Encode(T obj){
            return m_dataCoder.Encode(obj);
        }

        public void Post(T obj)
        {
            
        }

        public void ShutDown()
        {

        }

        public void Dispose()
        {
            
        }

        void ISocketServer.Receive(byte[] bytes)
        {
            
        }

        
    }
}

