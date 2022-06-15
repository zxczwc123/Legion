// ========================================================
// 描 述：UIPressButton.cs 
// 作 者： 
// 时 间：2020/03/03 14:16:45 
// 版 本：2019.2.1f1 
// ========================================================
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework.Core.UI {
    /// <summary>
    /// 长按会一直触发按钮点击事件
    /// </summary>
    public class UIPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {


        public Action OnPointerPress;

        private bool m_isPointerDown;

        /// <summary>
        /// 是否触发update 中 事件 如果触发,抬起手的时候不在触发事件 ，未触发，则抬起手的时候触发事件
        /// </summary>
        private bool m_isPointerPressTrigger;

        /// <summary>
        /// 按住触发事件的间隔
        /// </summary>
        private float m_pressEventInterval = 0.1f;

        private float m_passTime;

        public void OnPointerDown(PointerEventData eventData) {
            m_isPointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (m_isPointerDown) {
                if (!m_isPointerPressTrigger) {
                    if (OnPointerPress != null) OnPointerPress();
                }
            }
            m_passTime = 0;
            m_isPointerPressTrigger = false;
            m_isPointerDown = false;
        }

        /// <summary>
        /// 秒为单位
        /// </summary>
        /// <param name="interval"></param>
        public void SetPressInterval(int interval) {
            m_pressEventInterval = interval;
        }

        public void Update() {
            if (!m_isPointerDown) {
                return;
            }
            m_passTime += Time.deltaTime;
            if (m_isPointerPressTrigger) {
                if (m_passTime > m_pressEventInterval) {
                    m_passTime -= m_pressEventInterval;
                    if (OnPointerPress != null) OnPointerPress();
                }
            } else {
                if (m_passTime > 0.5f) {
                    m_isPointerPressTrigger = true;
                    m_passTime = 0;
                    if (OnPointerPress != null) OnPointerPress();
                }
            }
        }
    }
}