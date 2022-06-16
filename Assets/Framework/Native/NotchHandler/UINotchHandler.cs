// ========================================================
// 描 述：UINotchHandler.cs 
// 作 者： 
// 时 间：2020/01/16 21:15:15 
// 版 本：2019.2.1f1 
// ========================================================
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework.Core.UI {
    /// <summary>
    /// 画布适配器
    /// 适配不同手机，全面屏的刘海问题
    /// </summary>
    public class UINotchHandler : UIBehaviour {

        /// <summary>
        /// 手机屏幕长边与短边的比例 
        /// </summary>
        internal static float radio;

        /// <summary>
        /// 适配的边距
        /// x 上边距  y 下边距
        /// </summary>
        /// <returns></returns>
        private Vector2 offset = new Vector2 (45, 0);

        protected override void Start () {
            var rectViewRoot = (RectTransform)transform;
            Vector2 size = new Vector2 (Screen.width, Screen.height);
            if (Screen.orientation == ScreenOrientation.Portrait) {
                radio = size.y / size.x;
            } else {
                radio = size.x / size.y;
            }
            DoAdapte ();
        }

        internal void DoAdapte () {

#if UNITY_EDITOR
            if (radio >= 2.05) {
                if (Screen.orientation == ScreenOrientation.Portrait) {
                    RectTransform rectTransform = transform as RectTransform;
                    rectTransform.offsetMin = new Vector2 (0, offset.y);
                    rectTransform.offsetMax = new Vector2 (0, -offset.x);
                } else {
                    RectTransform rectTransform = transform as RectTransform;
                    rectTransform.offsetMin = new Vector2 (offset.x, 0);
                    rectTransform.offsetMax = new Vector2 (-offset.y, 0);
                }
            }
#elif UNITY_IOS
            if (radio >= 2.05) {
                if (Screen.orientation == ScreenOrientation.Portrait) {
                    RectTransform rectTransform = transform as RectTransform;
                    rectTransform.offsetMin = new Vector2 (0, offset.y);
                    rectTransform.offsetMax = new Vector2 (0, -offset.x);
                } else {
                    RectTransform rectTransform = transform as RectTransform;
                    rectTransform.offsetMin = new Vector2 (offset.x, 0);
                    rectTransform.offsetMax = new Vector2 (-offset.y, 0);
                }
            }
#elif UNITY_ANDROID
            if (radio >= 2.05) {
                if (Screen.orientation == ScreenOrientation.Portrait) {
                    RectTransform rectTransform = transform as RectTransform;
                    rectTransform.offsetMin = new Vector2 (0, offset.y);
                    rectTransform.offsetMax = new Vector2 (0, -offset.x);
                } else {
                    RectTransform rectTransform = transform as RectTransform;
                    rectTransform.offsetMin = new Vector2 (offset.x, 0);
                    rectTransform.offsetMax = new Vector2 (-offset.y, 0);
                }
            }
            //var NotchHandlerClassName = "com.unity3d.notch.NotchHandler";
            //AndroidJavaClass splashHandlerClass = new AndroidJavaClass (NotchHandlerClassName);
            //var hasNotch = splashHandlerClass.CallStatic<bool> ("hasNotchScreen");
            //if(hasNotch){
            //    if (Screen.orientation == ScreenOrientation.Portrait) {
            //        RectTransform rectTransform = transform as RectTransform;
            //        rectTransform.offsetMin = new Vector2 (0, offset.y);
            //        rectTransform.offsetMax = new Vector2 (0, -offset.x);
            //    } else {
            //        RectTransform rectTransform = transform as RectTransform;
            //        rectTransform.offsetMin = new Vector2 (offset.x, 0);
            //        rectTransform.offsetMax = new Vector2 (-offset.y, 0);
            //    }
            //}
#endif
        }

    }
}