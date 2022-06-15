// ========================================================
// 描 述：UIToggleGroup.cs 
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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Core.UI {
    public class UIToggleGroup : MonoBehaviour {

        public Action<int> OnValueChange;

        [SerializeField]
        private int m_defaultIndex;

        [SerializeField]
        private List<UIToggle> m_toggles;
        /// <summary>
        /// 是否激活
        /// </summary>
        private bool m_isAwake;

        public void Awake () {
            if (m_toggles != null && m_toggles.Count > 0) {
                for (var i = 0; i < m_toggles.Count;i++){
                    var toggle = m_toggles[i];
                    toggle.SetGroup(this);
                    toggle.SetActive(i == m_defaultIndex);
                }
            }
        }

        internal void OnToggleActive (UIToggle toggle) {
            var index = 0;
            for (var i = 0; i < m_toggles.Count; i++) {
                var tog = m_toggles[i];
                if(tog == toggle){
                    index = i;
                    break;
                }
            }
            if (m_defaultIndex == index) {
                return;
            }
            m_defaultIndex = index;
            if(OnValueChange != null){
                OnValueChange(m_defaultIndex);
            }
            if (m_toggles == null) {
                return;
            }
            for (var i = 0; i < m_toggles.Count; i++) {
                var tog = m_toggles[i];
                if(tog != toggle){
                    tog.SetActive (i == m_defaultIndex);
                }
            }
        }

        public int GetActiveToggleIndex() {
            return m_defaultIndex;
        }

        /// <summary>
        /// 手动设置索引 
        /// </summary>
        /// <param name="index"></param>
        public void SetToggleActive (int index) {
            if (m_defaultIndex == index) {
                return;
            }
            m_defaultIndex = index;
            if(OnValueChange != null){
                OnValueChange(m_defaultIndex);
            }
            if (m_isAwake) {
                return;
            }
            if (m_toggles == null) {
                return;
            }
            for (var i = 0; i < m_toggles.Count; i++) {
                var toggle = m_toggles[i];
                toggle.SetActive (i == m_defaultIndex);
            }
        }
    }
}