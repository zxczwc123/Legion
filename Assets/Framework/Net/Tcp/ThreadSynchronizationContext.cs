// ========================================================
// 描 述：NetThreadManger.cs 
// 创 建：高辉 
// 时 间：2021/11/22 15:51:48 
// 版 本：2020.3.18f1c1 
// ========================================================

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Framework.SocketNet {
    public sealed class ThreadSynchronizationContext : SynchronizationContext {
        public static ThreadSynchronizationContext Instance {
            get;
        } = new ThreadSynchronizationContext(Thread.CurrentThread.ManagedThreadId);

        private int _threadID;

        private ConcurrentQueue<Action> _actions;

        private Action _action;

        private ThreadSynchronizationContext(int threadID) {
            this._threadID = threadID;
            this._actions = new ConcurrentQueue<Action>();
        }

        public void Post(Action action) {
            if (this._threadID == Thread.CurrentThread.ManagedThreadId) {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
            this._actions.Enqueue(action);
        }

        public void Update() {
            if (!this._actions.TryDequeue(out _action)) {
                return;
            }
            try {
                _action();
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }

        public void PostNext(Action action) {
            this._actions.Enqueue(action);
        }
    }
}
