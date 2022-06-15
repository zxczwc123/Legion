// ========================================================
// 描 述：AsyncSocketClient 17-29-13-130.cs 
// 作 者：郑贤春 
// 时 间：2019/06/15 17:32:19 
// 版 本：2018.3.12f1 
// ========================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Framework.Net {
    public delegate void Callback (object data);

    public class AsyncSocketClient<T> : ISocketClient, IDisposable {

        SocketClientHandler clientHandler;

        Dictionary<IRequestHandler, Callback> mHandlerDict;

        private AsyncSocketDataCoder<T> m_dataCoder;

        public AsyncSocketClient () {
            if (clientHandler == null) {
                clientHandler = new SocketClientHandler ();
                clientHandler.SetSocketClient (this);
            }
        }

        /// <summary>
        /// 设置数据编码器
        /// </summary>
        /// <param name="dataCoder"></param>
        public void SetDataCoder (AsyncSocketDataCoder<T> dataCoder) {
            m_dataCoder = dataCoder;
        }

        private T Decode (byte[] bytes) {
            if (m_dataCoder == null) {
                throw new Exception ("socket client must SetDataCoder");
            }
            return m_dataCoder.Decode (bytes);
        }

        private byte[] Encode (T obj) {
            if (m_dataCoder == null) {
                throw new Exception ("socket client must SetDataCoder");
            }
            return m_dataCoder.Encode (obj);
        }

        public void Post (T obj) {
            byte[] bytes = Encode (obj);
            clientHandler.SendMessage (bytes);
        }

        public void Get (IRequestHandler send, Callback callback) {
            if (mHandlerDict == null) {
                mHandlerDict = new Dictionary<IRequestHandler, Callback> ();
            }
            mHandlerDict.Add (send, callback);
        }

        public void Get (IRequestHandler requestHandler, IResponseHandler responseHandler) {
            byte[] bytes = requestHandler.Code ();
            clientHandler.SendMessage (bytes);
        }

        public void Post (IRequestHandler send) {
            byte[] bytes = send.Code ();
            clientHandler.SendMessage (bytes);
        }

        public void Dispose () {

        }

        public void ShutDown () {
            clientHandler.Shutdown ();
        }

        void ISocketClient.Receive (byte[] bytes) {
            object data = Decode (bytes);
            if (this.mHandlerDict == null) return;
            foreach (IRequestHandler handler in this.mHandlerDict.Keys) {
                if (handler.IsRequest ()) {
                    this.mHandlerDict[handler] (data);
                    this.mHandlerDict.Remove (handler);
                    return;
                }
            }
        }
    }
}