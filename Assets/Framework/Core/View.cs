// ========================================================
// 描 述：View.cs 
// 作 者：郑贤春 
// 时 间：2019/06/18 15:08:50 
// 版 本：2018.3.12f1 
// ========================================================
using System;
using System.Collections;
using DG.Tweening;
using Framework.Core.MonoBehaviourAdapter;
using UnityEngine;

namespace Framework.Core {

    public enum AnimationShowType {
        ShowScale,
        ShowFromUp,
        ShowFromDown,
        ShowFromLeft,
        ShowFromRight,
    }

    public enum AnimationHideType {
        HideScale,
        HideToUp,
        HideToDown,
        HideToLeft,
        HideToRight,
    }

    /// <summary>
    /// 此view 为canvas 下的界面
    /// 次类已由 热更下的uiview 替换
    /// </summary>
    [Obsolete]
    public class View {
        public RectTransform rectTransform {
            private set;
            get;
        }

        public Transform transform {
            private set;
            get;
        }

        public GameObject gameObject {
            private set;
            get;
        }

        private MonoBehaviour _coroutineBehaiour;

        public MonoBehaviour CoroutineBehaiour {
            get {
                if(this._coroutineBehaiour == null) {
                    this._coroutineBehaiour = this.gameObject.AddComponent<UpdateAdapter>();
                }
                return this._coroutineBehaiour;
            }
        }

        public View () {
            
        }

        public View (RectTransform rectTranform) {
            if(rectTranform.parent != ViewManager.Instance.viewRoot) {
                throw new Exception(string.Format("{0} not view transform" , this.GetType().Name));
            }
            this.transform = rectTranform;
            this.rectTransform = rectTranform;
            this.gameObject = rectTranform.gameObject;
        }

        /// <summary>
        /// 显示界面
        /// </summary>
        public void Show () {
            ViewManager.Instance.OnViewShow (this);
        }

        /// <summary>
        /// 动画展示界面
        /// </summary>
        /// <param name="animationType"></param>
        public void Show (AnimationShowType animationType, Action onCompleted) {
            ViewManager.Instance.OnViewShow (this, animationType, 0.4f, onCompleted, Ease.Unset, 0);
        }

        /// <summary>
        /// 动画展示界面
        /// </summary>
        /// <param name="animationType"></param>
        public void Show (AnimationShowType animationType, float duration, Ease ease, float delay = 0f) {
            ViewManager.Instance.OnViewShow (this, animationType, duration, null, ease, delay);
        }

        /// <summary>
        /// 动画展示界面
        /// </summary>
        /// <param name="animationType"></param>
        public void Show (AnimationShowType animationType, Action onCompleted = null, float duration = 0.4f, Ease ease = Ease.Unset, float delay = 0) {
            ViewManager.Instance.OnViewShow (this, animationType, duration, onCompleted, ease, delay);
        }

        /// <summary>
        /// 以没有遮罩的形式显示界面
        /// </summary>
        public void ShowAsNoMask () {
            ViewManager.Instance.OnViewShowAsNoMask (this);
        }

        /// <summary>
        /// 隐藏界面
        /// </summary>
        public void Hide () {
            if(!this.transform.gameObject.activeSelf)
                return;
            ViewManager.Instance.OnViewHide (this);
        }

        /// <summary>
        /// 动画隐藏界面
        /// </summary>
        /// <param name="animationType"></param>
        public void Hide (AnimationHideType animationType, Action onCompleted) {
            if(!this.transform.gameObject.activeSelf)
                return;
            ViewManager.Instance.OnViewHide (this, animationType, 0.4f, null, Ease.Unset, 0);
        }

        /// <summary>
        /// 动画隐藏界面
        /// </summary>
        /// <param name="animationType"></param>
        public void Hide (AnimationHideType animationType, float duration, Ease ease, float delay = 0f) {
            if(!this.transform.gameObject.activeSelf)
                return;
            ViewManager.Instance.OnViewHide (this, animationType, duration, null, ease, delay);
        }

        /// <summary>
        /// 动画隐藏界面
        /// </summary>
        /// <param name="animationType"></param>
        public void Hide (AnimationHideType animationType, float duration = 0.4f, Ease ease = Ease.Unset, float delay = 0, Action onCompleted = null) {
            if(!this.transform.gameObject.activeSelf)
                return;
            ViewManager.Instance.OnViewHide (this, animationType, duration, onCompleted, ease, delay);
        }

        protected void StartCoroutine(IEnumerator routine){
            this.CoroutineBehaiour.StartCoroutine(routine);
        }

        protected void StopCoroutine(IEnumerator routine){
            this.CoroutineBehaiour.StopCoroutine(routine);
        }
    }
}