// ========================================================
// 描 述：UIStandardBezierLine2.cs 
// 作 者： 
// 时 间：2020/03/26 11:08:14 
// 版 本：2019.2.1f1 
// ========================================================
// ========================================================
// 描 述：UIStandardBezierLine.cs 
// 作 者： 
// 时 间：2019/07/21 22:14:23 
// 版 本：2018.3.12f1 
// ========================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UIShape
{
    /// <summary>
    /// 包括
    /// </summary>
    public class UIStandardBezierLine2 : RawImage {

        public enum MeshDisposition {
            Continuous,
            Fragmented
        }

        public enum PointCount {
            Three,
            Four
        }

        [SerializeField]
        private MeshDisposition meshDisposition = MeshDisposition.Fragmented;
        [SerializeField]
        private float m_lineWidth = 0.25f;
        [SerializeField]
        private int m_pointCount = 8;
        /// <summary>
        ///  路径点
        /// </summary>
        [SerializeField]
        private List<Vector3> m_pathList;
        private MeshDisposition m_preMeshDisposition;
        private int m_prePointCount;
        private float m_preLineWidth;
        private UIStandardBezierLinePointController m_pointController;

        private Mesh m_mesh;

        private bool m_isInitialized;

        protected override void Start () {
            base.Start ();
            Init ();
        }

        void LateUpdate () {
            UpdateMesh ();
        }

        public void Init () {
            if(m_isInitialized){
                return;
            }
            m_isInitialized = true;
            this.m_pointController = new UIStandardBezierLinePointController ();
            var rect = GetPixelAdjustedRect ();
            if (m_pathList == null) {
                m_pathList = new List<Vector3> ();
                m_pathList.Add (new Vector3 (-rect.width * 0.5f, 0));
                m_pathList.Add (new Vector3 (0f, 0));
                m_pathList.Add (new Vector3 (rect.width * 0.5f, 0));
            }
            UpdatePath ();
        }

        public void SetLineWidth (float lineWidth) {
            this.m_lineWidth = lineWidth;
        }

        public void SetPointCount (int count) {
            this.m_pointCount = count;
        }

        public void SetPath (List<Vector3> path) {
            if(m_pointController == null){
                Init();
            }
            this.m_pathList = path;
            UpdatePath ();
        }

        public List<Vector3> GetPath () {
            return this.m_pathList;
        }

        public void UpdateMesh () {

            bool updateFlag = false;
            if (m_preMeshDisposition != meshDisposition) {
                m_preMeshDisposition = meshDisposition;
                updateFlag = true;
            }
            if (m_prePointCount != m_pointCount) {
                m_prePointCount = m_pointCount;
                updateFlag = true;
            }
            if (m_preLineWidth != m_lineWidth) {
                m_preLineWidth = m_lineWidth;
                updateFlag = true;
            }
            if (updateFlag) {
                UpdatePath();
            }
        }

        public void UpdatePath () {
            if(m_pointController == null){
                Init();
            }
            m_pointCount = Mathf.Max (8, m_pointCount);
            m_pointController.UpdatePath (this.m_pathList, m_pointCount);
            m_pointController.UpdateMeshPath (this.m_lineWidth);
            var path = m_pointController.pathPositions;
            var pathUp = m_pointController.upPathPositions;
            var pathDown = m_pointController.downPathPositions;
            m_mesh = CreateMesh (path, pathUp, pathDown);
        }

        Mesh CreateMesh (Vector3[] path, Vector3[] upPath, Vector3[] downPath) {
            int length = upPath.Length;
            // 三角形顶点的坐标数组    
            Vector3[] vertices = new Vector3[4 * (length - 1)];
            // 三角形顶点UV坐标  
            Vector2[] uv = new Vector2[vertices.Length];
            // 三角形顶点ID数组    
            int[] triangles = new int[2 * (length - 1) * 3];
            // 点的的顺逆时针决定了网格显示方向
            float lastDistance = 0;
            for (int i = 0; i < length - 1; i++) {
                float distance = lastDistance + Vector3.Distance (path[i], path[i + 1]);
                triangles[6 * i + 0] = 4 * i + 2;
                triangles[6 * i + 1] = 4 * i + 1;
                triangles[6 * i + 2] = 4 * i + 0;
                triangles[6 * i + 3] = 4 * i + 1;
                triangles[6 * i + 4] = 4 * i + 2;
                triangles[6 * i + 5] = 4 * i + 3;
                vertices[4 * i + 0] = downPath[i];
                vertices[4 * i + 1] = downPath[i + 1];
                vertices[4 * i + 2] = upPath[i];
                vertices[4 * i + 3] = upPath[i + 1];
                if (meshDisposition == MeshDisposition.Continuous) {
                    uv[4 * i + 0] = new Vector2 (lastDistance / m_lineWidth, 0f);
                    uv[4 * i + 1] = new Vector2 (distance / m_lineWidth, 0f);
                    uv[4 * i + 2] = new Vector2 (lastDistance / m_lineWidth, 1f);
                    uv[4 * i + 3] = new Vector2 (distance / m_lineWidth, 1f);
                } else if (meshDisposition == MeshDisposition.Fragmented) {
                    uv[4 * i + 0] = new Vector2 (0f, 0f);
                    uv[4 * i + 1] = new Vector2 (1f, 0f);
                    uv[4 * i + 2] = new Vector2 (0f, 1f);
                    uv[4 * i + 3] = new Vector2 (1f, 1f);
                }
                lastDistance = distance;
            }
            Mesh mesh = new Mesh ();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            return mesh;
        }

        protected override void OnPopulateMesh (VertexHelper vh) {
            Material m = this.material;
            var rect = GetPixelAdjustedRect ();
            vh.Clear ();
            // 左边柱子
            if (m_mesh != null) {
                var vertices = m_mesh.vertices;
                var triangles = m_mesh.triangles;
                var uvs = m_mesh.uv;
                for (var i = 0; i < vertices.Length; i++) {
                    var vertice = vertices[i];
                    var uv = uvs[i];
                    vh.AddVert (vertice, color, uv);
                }
                for (var i = 0; i < triangles.Length; i += 3) {
                    vh.AddTriangle (triangles[i], triangles[i + 1], triangles[i + 2]);
                }
            }
        }

    }

}