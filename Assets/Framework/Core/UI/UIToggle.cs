// ========================================================
// 描 述：UIToggle.cs 
// 作 者： 
// 时 间：2020/01/04 11:43:40 
// 版 本：2019.2.1f1 
// ========================================================
// ========================================================
// 描 述：UIToggleGroupAdapter.cs 
// 作 者： 
// 时 间：2020/01/04 10:39:33 
// 版 本：2019.2.1f1 
// ========================================================
// ========================================================
// 描 述：UIAudioClipPlayer.cs 
// 作 者：郑贤春 
// 时 间：2019/06/19 18:00:50 
// 版 本：2018.3.12f1 
// ========================================================
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.Core.UI
{
    public class UIToggle : Selectable, IPointerClickHandler {

        [SerializeField]
        private bool m_isOn;

        [SerializeField]
        private GameObject m_goOn;

        [SerializeField]
        private GameObject m_goOff;
        /// <summary>
        /// 组
        /// </summary>
        private UIToggleGroup m_group;

        public Action<bool> OnValueChanged;

        protected override void Awake () {
            if(m_goOn != null){
                m_goOn.SetActive(m_isOn);
            }
            if(m_goOff != null){
                m_goOff.SetActive(!m_isOn);
            }
        }

        internal void SetGroup(UIToggleGroup group){
            m_group = group;
        }

        public void SetActive(bool isOn){
            if(m_isOn == isOn){
                return;
            }
            m_isOn = isOn;
            if (OnValueChanged != null) {
                OnValueChanged (m_isOn);
            }
            if(m_goOn != null){
                m_goOn.SetActive(m_isOn);
            }
            if(m_goOff != null){
                m_goOff.SetActive(!m_isOn);
            }
        }

        public void OnPointerClick (PointerEventData eventData) {
            if(m_isOn && m_group != null){
                // 有组情况下不允许手动关闭
                return;
            }
            m_isOn = !m_isOn;
            if (OnValueChanged != null) {
                OnValueChanged (m_isOn);
            }
            if(m_goOn != null){
                m_goOn.SetActive(m_isOn);
            }
            if(m_goOff != null){
                m_goOff.SetActive(!m_isOn);
            }
            if(m_group != null && m_isOn){
                m_group.OnToggleActive(this);
            }
        }

    }
}