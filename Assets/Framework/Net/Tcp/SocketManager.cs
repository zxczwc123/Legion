// ========================================================
// 描 述：SocketManager.cs 
// 功 能：Socket封装，应该算剥离的比较干净了，只做消息的发送以及接受，具体处理业务实现
// 创 建：高辉 
// 时 间：2021/11/22 16:32:08 
// 版 本：2020.3.18f1c1 
// ========================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Framework.Core;
using UnityEngine;

namespace Framework.SocketNet {
    public sealed class SocketManager : Manager<SocketManager> {

        public IPEndPoint RemoteAddress { get; private set; }

        public bool IsDisposed {
            get;
            private set;
        }

        public Channel Creat(IPEndPoint ip,Action<int,IPEndPoint> aError, Action<byte[],IPEndPoint> aRead,Action callback) {
            var channel = new Channel(ip, callback);
            channel.SetAction(aError,aRead);
            return channel;
        }

        private void OnDestroy() {
            this.Dispose();
        }

        public override void Dispose() {
            
        }
    }
}
