using UnityEngine;
using System.Collections;
using System.Threading;
using System;

namespace Framework.Os
{
    public class Looper : MonoBehaviour
    {
        private static Looper mLooper = null;
        public static Looper Instance
        {
            get
            {
                Initialize();
                return mLooper;
            }
        }

        static bool initialized;
        static void Initialize()
        {
            if (!initialized)
            {
                if (!Application.isPlaying) return;
                initialized = true;
                GameObject looper = new GameObject("Looper");
                mLooper = looper.AddComponent<Looper>();
            }
        }

        public static MessageQueue myQueue()
        {
            return mLooper.mQueue;
        }

        internal MessageQueue mQueue;

        void Awake()
        {
            initialized = true;
            mLooper = this;
            mQueue = new MessageQueue();
            StartCoroutine(Loop());
        }

        IEnumerator Loop()
        {
            if (mLooper == null)
            {
                throw new ArgumentNullException("No Looper!");
            }
            MessageQueue queue = mLooper.mQueue;
            while (true)
            {
                Message msg = queue.Next();
                while (msg != null)
                {
                    if (msg.mCallback != null)
                    {
                        msg.mCallback();
                    }
                    else
                    {
                        if (msg.target != null)
                        {
                            msg.target.mOnCallback(msg);
                        }
                        else
                        {
                            throw new Exception("Unknown Exception");
                        }
                    }
                    msg = queue.Next();
                }
                yield return new WaitForSeconds(0);
            }
        }
    }
}