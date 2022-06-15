// ========================================================
// 描 述：UI.cs 
// 作 者：郑贤春 
// 时 间：2017/03/01 08:17:43 
// 版 本：5.4.1f1 
// ========================================================
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UIShape {

    public class UILadder : Image {
        [SerializeField]
        private float m_width = 10f;
        [SerializeField]
        private float m_divider = 20f;

        protected override void OnPopulateMesh (VertexHelper vh) {
            var rect = GetPixelAdjustedRect ();
            var uv = (overrideSprite != null) ? UnityEngine.Sprites.DataUtility.GetOuterUV (overrideSprite) : Vector4.zero;
            vh.Clear ();
            // 左边柱子
            Vector4 leftUV = new Vector4 (Mathf.Lerp (uv.x, uv.z, 0f), Mathf.Lerp (uv.y, uv.w, 0f), Mathf.Lerp (uv.x, uv.z, this.m_width / rect.width), Mathf.Lerp (uv.y, uv.w, 1f));
            vh.AddVert (new Vector3 (rect.x, rect.y), color, new Vector2 (leftUV.x, leftUV.y));
            vh.AddVert (new Vector3 (rect.x, rect.y + rect.height), color, new Vector2 (leftUV.x, leftUV.w));
            vh.AddVert (new Vector3 (rect.x + this.m_width, rect.y + rect.height), color, new Vector2 (leftUV.z, leftUV.w));
            vh.AddVert (new Vector3 (rect.x + this.m_width, rect.y), color, new Vector2 (leftUV.z, leftUV.y));
            vh.AddTriangle (0, 1, 2);
            vh.AddTriangle (2, 3, 0);
            // 右边柱子
            Vector4 rightUV = new Vector4 (Mathf.Lerp (uv.x, uv.z, 1 - this.m_width / rect.width), Mathf.Lerp (uv.y, uv.w, 0f), Mathf.Lerp (uv.x, uv.z, 1f), Mathf.Lerp (uv.y, uv.w, 1f));
            vh.AddVert (new Vector3 (rect.x + rect.width - this.m_width, rect.y), color, new Vector2 (rightUV.x, rightUV.y));
            vh.AddVert (new Vector3 (rect.x + rect.width - this.m_width, rect.y + rect.height), color, new Vector2 (rightUV.x, rightUV.w));
            vh.AddVert (new Vector3 (rect.x + rect.width, rect.y + rect.height), color, new Vector2 (rightUV.z, rightUV.w));
            vh.AddVert (new Vector3 (rect.x + rect.width, rect.y), color, new Vector2 (rightUV.z, rightUV.y));
            vh.AddTriangle (4, 5, 6);
            vh.AddTriangle (6, 7, 4);
            // 中间踏板
            int stepCount = (int) (rect.height / this.m_divider);
            for (int i = 0; i < stepCount; i++) {
                float heightuv = -rect.height * 0.5f + (rect.height - (stepCount - 1) * m_divider - this.m_width) * 0.5f + i * this.m_divider;
                float height = rect.y + (rect.height - (stepCount - 1) * m_divider - this.m_width) * 0.5f + i * this.m_divider;
                Vector4 stepUV = new Vector4 (Mathf.Lerp (uv.x, uv.z, this.m_width / rect.width), Mathf.Lerp (uv.y, uv.w, (heightuv + rect.height * 0.5f) / rect.height), Mathf.Lerp (uv.x, uv.z, 1 - this.m_width / rect.width), Mathf.Lerp (uv.y, uv.w, (heightuv + this.m_width + rect.height * 0.5f) / rect.height));
                vh.AddVert (new Vector3 (-rect.width * 0.5f + this.m_width, height), color, new Vector2 (stepUV.x, stepUV.y));
                vh.AddVert (new Vector3 (-rect.width * 0.5f + this.m_width, height + this.m_width), color, new Vector2 (stepUV.x, stepUV.w));
                vh.AddVert (new Vector3 (+rect.width * 0.5f - this.m_width, height + this.m_width), color, new Vector2 (stepUV.z, stepUV.w));
                vh.AddVert (new Vector3 (+rect.width * 0.5f - this.m_width, height), color, new Vector2 (stepUV.z, stepUV.y));
                vh.AddTriangle (8 + i * 4, 9 + i * 4, 10 + i * 4);
                vh.AddTriangle (10 + i * 4, 11 + i * 4, 8 + i * 4);
            }
        }
    }

}