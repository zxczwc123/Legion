// ========================================================
// 描 述：UIClickListener.cs 
// 作 者： 
// 时 间：2019/07/29 14:11:18 
// 版 本：2018.3.12f1 
// ========================================================
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Framework.Core.UI {
    [RequireComponent (typeof (UnityEngine.UI.Selectable))]
    public sealed class UIClickListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        public UnityEvent onLongClick {
            get;
            private set;
        }

        public UnityEvent onDoubleClick {
            get;
            private set;
        }

        public UnityEvent onSingleClick {
            get;
            private set;
        }

        /// <summary>
        /// 双击需要的最小时间间隔
        /// </summary>
        private float m_DoubleClickInterval = 0.2f * 10000000;

        /// <summary>
        /// 长按所需的时间
        /// </summary>
        private float m_LongClickTime = 1.0f * 10000000;

        private long m_singleClickStartTime;

        private long m_singleClickEndTime;

        private long m_singleClickTime;

        private int m_clickCount;

        void Awake () {
            onDoubleClick = new UnityEvent ();
            onSingleClick = new UnityEvent ();
            onLongClick = new UnityEvent ();
        }

        void Update () {
            if (m_clickCount > 0) {
                var nowTime = DateTime.Now.Ticks;
                var passTime = nowTime - m_singleClickEndTime;
                if (passTime > m_DoubleClickInterval) {
                    // 单击成立
                    if (m_singleClickTime > m_LongClickTime) {
                        // 长单击
                        Debug.Log("长单击");
                        if (onLongClick != null) {
                            onLongClick.Invoke ();
                        }
                    } else {
                        // 单击
                        Debug.Log("单击");
                        if (onSingleClick != null) {
                            onSingleClick.Invoke ();
                        }
                    }
                    m_clickCount = 0;
                }
            }
        }

        public void OnPointerDown (PointerEventData eventData) {
            if (m_clickCount == 0) {
                // 第一次点击
                m_singleClickStartTime = DateTime.Now.Ticks;
            } else {
                // 第二次点击开始 
                m_singleClickStartTime = DateTime.Now.Ticks;
            }
        }

        /// <summary>
        /// 一次长单击 加一次短单击 会触发双击
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp (PointerEventData eventData) {
            if (m_clickCount == 0) {
                // 第一次点击结束
                m_singleClickEndTime = DateTime.Now.Ticks;
                m_singleClickTime = m_singleClickEndTime - m_singleClickStartTime;
                m_clickCount = 1;
            } else {
                // 第二次点击结束 有一次点击积累 双击成立
                 Debug.Log("双击");
                if (onDoubleClick != null) {
                    onDoubleClick.Invoke ();
                }
                m_clickCount = 0;
            }
        }
    }
}