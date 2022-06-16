// ========================================================
// 描 述：EventManager.cs 
// 作 者： 
// 时 间：2020/01/03 17:46:46 
// 版 本：2019.2.1f1 
// ========================================================

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public class EventManager : Manager<EventManager>
    {
        public static void Post(string eventName, Bundle bundle = null)
        {
            Instance.PostInternal(eventName, bundle);
        }

        public static void Register(string eventName, Action<Bundle> callback = null)
        {
            Instance.RegisterInternal(eventName, callback);
        }

        public static void UnRegister(string eventName, Action<Bundle> callback = null)
        {
            Instance.UnRegisterInternal(eventName, callback);
        }

        /// <summary>
        /// 事件表
        /// </summary>
        private Dictionary<string, Action<Bundle>> m_eventDict = new Dictionary<string, Action<Bundle>>();

        /// <summary>
        /// 发送的事件 
        /// </summary>
        private List<EventHolder> m_events = new List<EventHolder>();

        /// <summary>
        /// 待执行的事件 
        /// </summary>
        private List<EventHolder> m_waitEvents = new List<EventHolder>();

        /// <summary>
        /// 注册事件
        /// </summary>
        public void RegisterInternal(string eventName, Action<Bundle> callback)
        {
            if (!m_eventDict.ContainsKey(eventName))
            {
                m_eventDict.Add(eventName, callback);
            }
            else
            {
                m_eventDict[eventName] += callback;
            }
        }

        /// <summary>
        /// 反注册事件
        /// </summary>
        public void UnRegisterInternal(string eventName, Action<Bundle> callback)
        {
            if (!m_eventDict.ContainsKey(eventName))
            {
                Debug.Log(string.Format("eventName : {0} had not been registered", eventName));
                return;
            }
            if (m_eventDict.ContainsKey(eventName))
                m_eventDict[eventName] -= callback;
            if (m_eventDict[eventName] == null)
                m_eventDict.Remove(eventName);
        }

        /// <summary>
        /// 发送事件
        /// </summary>
        public void PostInternal(string eventName, Bundle bundle = null)
        {
            if (!m_eventDict.ContainsKey(eventName))
            {
                Debug.Log(string.Format("eventName : {0} had not been registered,post was canceled", eventName));
                return;
            }
            m_events.Add(new EventHolder() {eventName = eventName, bundle = bundle});
        }

        /// <summary>
        /// 更新 ，检测事件 执行事件
        /// </summary>
        private void Update()
        {
            if (m_events.Count > 0 && m_waitEvents.Count == 0)
            {
                m_waitEvents.AddRange(m_events);
                m_events.Clear();
                foreach (var eventHolder in m_waitEvents)
                {
                    var eventName = eventHolder.eventName;
                    var bundle = eventHolder.bundle;
                    if (!m_eventDict.ContainsKey(eventName))
                    {
                        continue;
                    }
                    var eventCallbacks = m_eventDict[eventName];
                    eventCallbacks?.Invoke(bundle);
                }
                m_waitEvents.Clear();
            }
        }

        private void OnDestroy()
        {
            m_eventDict.Clear();
        }

        /// <summary>
        /// 卸载管理器
        /// </summary>
        public override void Dispose()
        {
            m_eventDict.Clear();
        }

        public class EventHolder
        {
            public Bundle bundle;
            public string eventName;
        }
    }
}