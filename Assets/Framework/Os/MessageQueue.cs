using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

namespace Framework.Os
{
    public class MessageQueue
    {
        internal Message mMessages;
        private bool mQuiting;
        internal bool mQuitAllowed = true;
        private bool mBlocked;

        internal MessageQueue()
        {
        }

        internal Message Next()
        {
            long now = DateTime.Now.Ticks;
            Message msg = mMessages;
            if (msg != null)
            {
                long when = msg.when;
                if (now >= when)
                {
                    mMessages = msg.next;
                    msg.next = null;
                    return msg;
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        internal bool EnqueueMessage(Message msg, long when)
        {
            if (msg.target == null)
            {
                throw new ArgumentNullException("Main thread not allowed to quit");
            }
            lock (this)
            {
                if (mQuiting)
                {
                    return false;
                }
                else if (msg.target == null)
                {
                    mQuiting = true;
                }

                msg.when = when;
                //Log.d("MessageQueue", "Enqueing: " + msg);
                Message p = mMessages;
                if (p == null || when == 0 || when < p.when)
                {
                    msg.next = p;
                    mMessages = msg;
                }
                else
                {
                    Message prev = null;
                    while (p != null && p.when <= when)
                    {
                        prev = p;
                        p = p.next;
                    }
                    msg.next = prev.next;
                    prev.next = msg;
                }
            }
            return true;
        }
    }

}
