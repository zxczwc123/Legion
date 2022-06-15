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


    [RequireComponent (typeof (UnityEngine.UI.Selectable))]
    public class UIAudioClipPlayer : MonoBehaviour, IPointerClickHandler {

        /// <summary>
        /// 已经加载过的audioClip
        /// </summary>
        private static List<string> s_Clips;

        /// <summary>
        /// 要播放的音频路径
        /// </summary>
        [FormerlySerializedAs("clipPath")] [SerializeField]
        private string m_ClipPath;

        private void Awake () {
            if (s_Clips == null) {
                s_Clips = new List<string> ();
            }
            if (!s_Clips.Contains (m_ClipPath)) {
                s_Clips.Add (m_ClipPath);
                AudioManager.Instance.LoadAudioClip (m_ClipPath, m_ClipPath);
            }
        }

        #region IPointerClickHandler implementation
        public void OnPointerClick (PointerEventData eventData) {

            AudioManager.Instance.PlaySfx (m_ClipPath);
        }
        #endregion
    }
}