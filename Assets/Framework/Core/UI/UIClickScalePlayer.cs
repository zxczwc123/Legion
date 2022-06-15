// ========================================================
// 描 述：UIClickScalePlayer.cs 
// 作 者：郑贤春 
// 时 间：2019/06/19 22:57:58 
// 版 本：2018.3.12f1 
// ========================================================
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework.Core.UI {
    [RequireComponent (typeof (UnityEngine.UI.Selectable))]
    public sealed class UIClickScalePlayer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        [SerializeField]
        private Vector3 m_scale = new Vector3 (1.1f, 1.1f, 1.1f);
        [SerializeField]
        private float m_scaleDuration = 0.1f;
        [SerializeField]
        private Ease m_scaleEase = Ease.OutBack;
        private Vector3 m_scaleCurrent;

        void Awake () {
            m_scaleCurrent = transform.localScale;
        }

        public void OnPointerDown (PointerEventData eventData) {
            transform.DOScale (m_scale, m_scaleDuration).SetEase (m_scaleEase);
        }

        public void OnPointerUp (PointerEventData eventData) {
            transform.DOScale (m_scaleCurrent, m_scaleDuration);
        }
    }
}