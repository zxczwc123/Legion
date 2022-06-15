// ========================================================
// 描 述：UIBezierLine.cs 
// 作 者： 
// 时 间：2019/07/19 14:47:12 
// 版 本：2018.3.12f1 
// ========================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UIShape
{
    public class UIBezierLine : MaskableGraphic
    {
        public enum MeshDisposition
        {
            Continuous,
            Fragmented
        }

        [SerializeField] private MeshDisposition meshDisposition = MeshDisposition.Fragmented;
        [SerializeField] private float m_lineWidth = 0.25f;
        [SerializeField] private float m_dividerHeight = 0.25f;

        /// <summary>
        ///  路径点
        /// </summary>
        [SerializeField] private List<Vector3> m_pathList;

        private MeshDisposition m_preMeshDisposition;
        private float m_preDividerHeight;
        private float m_preLineWidth;
        private UIBezierLinePointController m_pointController;

        private Mesh m_mesh;

        protected override void Start()
        {
            base.Start();
            Init();
        }

        void LateUpdate()
        {
            UpdateMesh();
        }

        public void Init()
        {
            m_pointController = new UIBezierLinePointController();
            var rect = GetPixelAdjustedRect();
            if (m_pathList == null)
            {
                m_pathList = new List<Vector3>();
                m_pathList.Add(new Vector3(-rect.width * 0.5f, 0));
                m_pathList.Add(new Vector3(rect.width * 0.5f, 0));
            }
            UpdateMesh();
        }

        public void SetLineWidth(float lineWidth)
        {
            m_lineWidth = lineWidth;
        }

        public void SetDividerHeight(float height)
        {
            m_dividerHeight = height;
        }

        public void SetPath(List<Vector3> path)
        {
            m_pathList = path;
            UpdatePath();
        }

        public List<Vector3> GetPath()
        {
            return m_pathList;
        }

        public void UpdateMesh()
        {
            bool updateFlag = false;
            if (m_preMeshDisposition != meshDisposition)
            {
                m_preMeshDisposition = meshDisposition;
                updateFlag = true;
            }
            m_dividerHeight = Mathf.Max(0.001f, m_dividerHeight);
            if (m_preDividerHeight != m_dividerHeight)
            {
                m_preDividerHeight = m_dividerHeight;
                updateFlag = true;
                m_pointController.PointListAverage(m_pathList, m_dividerHeight);
            }
            if (m_preLineWidth != m_lineWidth)
            {
                m_preLineWidth = m_lineWidth;
                updateFlag = true;
            }
            if (updateFlag)
            {
                Vector3[] pathUp = m_pointController.GetPathUp(m_lineWidth);
                Vector3[] pathDown = m_pointController.GetPathDown(m_lineWidth);
                m_mesh = CreateMesh(pathUp, pathDown);
            }
        }

        public void UpdatePath()
        {
            m_pointController.PointListAverage(m_pathList, m_dividerHeight);
            Vector3[] pathUp = m_pointController.GetPathUp(m_lineWidth);
            Vector3[] pathDown = m_pointController.GetPathDown(m_lineWidth);
            m_mesh = CreateMesh(pathUp, pathDown);
        }

        Mesh CreateMesh(Vector3[] upPath, Vector3[] downPath)
        {
            int length = upPath.Length;
            // 三角形顶点的坐标数组    
            Vector3[] vertices = new Vector3[4 * (length - 1)];
            // 三角形顶点UV坐标  
            Vector2[] uv = new Vector2[vertices.Length];
            // 三角形顶点ID数组    
            int[] triangles = new int[2 * (length - 1) * 3];
            // 点的的顺逆时针决定了网格显示方向
            float lastDistance = 0;
            for (int i = 0; i < length - 1; i++)
            {
                float distance = lastDistance + m_dividerHeight;
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
                if (meshDisposition == MeshDisposition.Continuous)
                {
                    uv[4 * i + 0] = new Vector2(lastDistance / m_lineWidth, 0f);
                    uv[4 * i + 1] = new Vector2(distance / m_lineWidth, 0f);
                    uv[4 * i + 2] = new Vector2(lastDistance / m_lineWidth, 1f);
                    uv[4 * i + 3] = new Vector2(distance / m_lineWidth, 1f);
                }
                else if (meshDisposition == MeshDisposition.Fragmented)
                {
                    uv[4 * i + 0] = new Vector2(0f, 0f);
                    uv[4 * i + 1] = new Vector2(1f, 0f);
                    uv[4 * i + 2] = new Vector2(0f, 1f);
                    uv[4 * i + 3] = new Vector2(1f, 1f);
                }
                lastDistance = distance;
            }
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            return mesh;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (m_mesh != null)
            {
                var vertices = m_mesh.vertices;
                var triangles = m_mesh.triangles;
                var uvs = m_mesh.uv;
                for (var i = 0; i < vertices.Length; i++)
                {
                    var vertex = vertices[i];
                    var uv = uvs[i];
                    vh.AddVert(vertex, color, uv);
                }
                for (var i = 0; i < triangles.Length; i += 3)
                {
                    vh.AddTriangle(triangles[i], triangles[i + 1], triangles[i + 2]);
                }
            }
        }
    }
}