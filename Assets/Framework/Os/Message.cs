using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Framework.Os
{

    public delegate void Callback();
    public delegate void OnCallback(Message msg);

    public class Message
    {
        public int what;
        public object obj;

        internal long when;
        internal Handler target;
        internal Callback mCallback;
        internal Message next;
        private static object sPoolSync = new object();
        private static Message sPool;
        private static int sPoolSize = 0;

        private const int MAX_POOL_SIZE = 10;
        public static Message Obtain()
        {
            lock(sPoolSync) {
                if (sPool != null)
                {
                    Message m = sPool;
                    sPool = m.next;
                    m.next = null;
                    sPoolSize--;
                    return m;
                }
            }
            return new Message();
        }

        public static Message Obtain(Handler h)
        {
            Message m = Obtain();
            m.target = h;
            return m;
        }

        public static Message Obtain(Handler h, Callback callback)
        {
            Message m = Obtain();
            m.target = h;
            m.mCallback = callback;
            return m;
        }

        public static Message obtain(Handler h, int what)
        {
            Message m = Obtain();
            m.target = h;
            m.what = what;
            return m;
        }

        public static Message obtain(Handler h, int what, object obj)
        {
            Message m = Obtain();
            m.target = h;
            m.what = what;
            m.obj = obj;

            return m;
        }

        public long GetWhen()
        {
            return when;
        }

        public void SetTarget(Handler target)
        {
            this.target = target;
        }

        public Handler GetTarget()
        {
            return target;
        }

        public void SendToTarget()
        {
            target.SendMessage(this);
        }

        public Message()
        {
        }

        internal Message(Callback callback)
        {
            this.mCallback = callback;
        }

    }
}
