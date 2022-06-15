// ========================================================
// 描 述：UICanvasApdater.cs 
// 作 者：郑贤春 
// 时 间：2019/06/20 16:17:12 
// 版 本：2018.3.12f1 
// ========================================================
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
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Framework.Core.UI {
    /// <summary>
    /// 画布适配器
    /// 适配不同手机，全面屏的刘海问题
    /// </summary>
    public class UICanvasAdapter : UIBehaviour {

        /// <summary>
        /// 手机屏幕长边与短边的比例 
        /// </summary>
        internal static float s_Radio;

        /// <summary>
        /// 适配的边距
        /// x 上边距  y 下边距
        /// </summary>
        /// <returns></returns>
        [FormerlySerializedAs("offset")] [SerializeField]
        protected Vector2 m_Offset = new Vector2 (40, 40);

        protected override void Start () {
            var rectViewRoot = ViewManager.Instance.viewRoot;
            Vector2 size = rectViewRoot.rect.size;
            if (Screen.orientation == ScreenOrientation.Portrait) {
                s_Radio = size.y / size.x;
            } else {
                s_Radio = size.x / size.y;
            }
            DoUpdate ();
        }

        internal void DoUpdate () {
            if (s_Radio >= 2.05) {
                if (Screen.orientation == ScreenOrientation.Portrait) {
                    RectTransform rectTransform = transform as RectTransform;
                    rectTransform.offsetMin = new Vector2 (0, m_Offset.y);
                    rectTransform.offsetMax = new Vector2 (0, -m_Offset.x);
                } else {
                    RectTransform rectTransform = transform as RectTransform;
                    rectTransform.offsetMin = new Vector2 (m_Offset.x, 0);
                    rectTransform.offsetMax = new Vector2 (-m_Offset.y, 0);
                }
            }
        }

    }
}