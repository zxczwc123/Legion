// ========================================================
// 描 述：UIEnableScalePlayer.cs 
// 作 者：郑贤春 
// 时 间：2019/06/19 22:57:58 
// 版 本：2018.3.12f1 
// ========================================================
// ========================================================
// 描 述：UIButtonMotionScale.cs 
// 作 者：郑贤春 
// 时 间：2019/06/19 18:24:12 
// 版 本：2018.3.12f1 
// ========================================================
// ========================================================
// 描 述：UIAudioClipPlayer.cs 
// 作 者：郑贤春 
// 时 间：2019/06/19 18:00:50 
// 版 本：2018.3.12f1 
// ========================================================
using DG.Tweening;
using UnityEngine;

namespace Framework.Core.UI
{
    public class UIEnableScalePlayer : MonoBehaviour
    {
        [SerializeField]
        private Vector3 m_startScale = Vector3.one;
        [SerializeField]
        private Vector3 m_endScale = Vector3.one;
        [SerializeField]
        private float m_time = 0.5f;
        [SerializeField]
        private float m_delay = 0;
        [SerializeField]
        private Ease m_ease = Ease.OutBack;

        [SerializeField]
        private AnimationCurve m_curve;

        #region UIMotion implementation

        private Tween m_tween;

        public void DoMotion()
        {
            TryCloseTween();
            Reset();
            transform.localScale = m_startScale;
            m_tween = transform.DOScale(m_endScale, m_time).SetDelay(m_delay);
            if(m_curve != null){
                m_tween.SetEase(m_curve);
            }else{
                m_tween.SetEase(m_ease);
            }
        }

        public void StopMotion()
        {
            TryCloseTween();
        }

        public void Reset()
        {
            StopMotion();
            transform.localScale = m_endScale;
        }

        void TryCloseTween()
        {
            if (m_tween != null)
            {
                m_tween.Kill(true);
            }
            m_tween = null;
        }

        #endregion

        void OnEnable()
        {
            DoMotion();
        }

        void OnDisable()
        {
            Reset();
        }
    }
}