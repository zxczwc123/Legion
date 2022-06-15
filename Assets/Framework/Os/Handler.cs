using UnityEngine;
using System.Collections;
using System.Threading;
using System;

namespace Framework.Os
{
    public class Handler
    {
        MessageQueue mQueue;
        internal Looper mLooper;
        internal OnCallback mOnCallback;
        internal Callback mCallback;

        public Handler()
        {
            mLooper = Looper.Instance;
            mQueue = mLooper.mQueue;
            mOnCallback = null;
        }

        public Handler(OnCallback onCallback)
        {
            mLooper = Looper.Instance;
            mQueue = mLooper.mQueue;
            if (onCallback == null) throw new ArgumentNullException("Don't accept a null callback");
            mOnCallback = onCallback;
        }


        public bool Post(Callback callback)
        {
            return SendMessageDelayed(new Message(), 0);
        }

        public bool PostAtTime(Callback callback, long uptimeMillis)
        {
            return SendMessageAtTime(new Message(callback), uptimeMillis);
        }

        public bool PostDelayed(Callback callback, long delayMillis)
        {
            return SendMessageDelayed(new Message(callback), delayMillis);
        }

        public bool SendMessage(Message msg)
        {
            return SendMessageDelayed(msg, 0);
        }

        public bool SendMessageDelayed(Message msg, long delayMillis)
        {
            if (delayMillis < 0)
            {
                delayMillis = 0;
            }
            return SendMessageAtTime(msg, DateTime.Now.Ticks + delayMillis * 10000);
        }

        public bool SendMessageAtTime(Message msg, long uptimeMillis)
        {
            bool sent = false;
            MessageQueue queue = mQueue;
            if (queue != null)
            {
                msg.target = this;
                sent = queue.EnqueueMessage(msg, uptimeMillis);
            }
            return sent;
        }
    }
}
