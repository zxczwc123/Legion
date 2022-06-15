// ========================================================
// 描 述：UITriggle.cs 
// 作 者：郑贤春 
// 时 间：2017/03/03 21:12:53 
// 版 本：5.4.1f1 
// ========================================================
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UIShape {
    public class UITriangle : Image {
        [SerializeField]
        private int m_size;

        protected override void OnPopulateMesh (VertexHelper vh) {
            var rect = GetPixelAdjustedRect ();
            var uv = (overrideSprite != null) ? UnityEngine.Sprites.DataUtility.GetOuterUV (overrideSprite) : Vector4.zero;
            vh.Clear ();
            if (this.m_size == 0) return;
            float divider = rect.width / this.m_size;
            for (int i = 0; i < this.m_size; i++) {
                vh.AddVert (new Vector3 (rect.x + (i + 0.0f) * divider, rect.y), color, new Vector2 (Mathf.Lerp (uv.x, uv.z, 0.0f), Mathf.Lerp (uv.y, uv.w, 0.0f)));
                vh.AddVert (new Vector3 (rect.x + (i + 1.0f) * divider, rect.y), color, new Vector2 (Mathf.Lerp (uv.x, uv.z, 1.0f), Mathf.Lerp (uv.y, uv.w, 0.0f)));
                vh.AddVert (new Vector3 (rect.x + (i + 0.5f) * divider, rect.y + rect.height), color, new Vector2 (Mathf.Lerp (uv.x, uv.z, 0.5f), Mathf.Lerp (uv.y, uv.w, 1.0f)));
                vh.AddTriangle (i * 3, i * 3 + 2, i * 3 + 1);
            }
        }
    }

}