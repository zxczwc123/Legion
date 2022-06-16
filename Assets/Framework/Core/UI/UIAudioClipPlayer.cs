// ========================================================
// 描 述：UIAudioClipPlayer.cs 
// 作 者：郑贤春 
// 时 间：2019/06/19 18:00:50 
// 版 本：2018.3.12f1 
// ========================================================
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Framework.Core.UI
{

    public delegate void OnAudioPlayEvent(string clipPath);
    
    public delegate void OnAudioLoadEvent(string clipPath);

    [RequireComponent (typeof (UnityEngine.UI.Selectable))]
    public class UIAudioClipPlayer : MonoBehaviour, IPointerClickHandler {

        public static OnAudioPlayEvent onAudioPlayEvent;
        
        public static OnAudioPlayEvent onAudioLoadEvent;

        /// <summary>
        /// 要播放的音频路径
        /// </summary>
        [SerializeField]
        private string m_ClipPath;

        private void Awake () {
            onAudioLoadEvent?.Invoke(m_ClipPath);
        }

        #region IPointerClickHandler implementation
        public void OnPointerClick (PointerEventData eventData) {
            onAudioPlayEvent?.Invoke(m_ClipPath);
        }
        #endregion
    }
}