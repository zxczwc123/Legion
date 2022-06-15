// ========================================================
// 描 述：UIBezierLinePointController.cs 
// 作 者： 
// 时 间：2019/07/19 14:47:12 
// 版 本：2018.3.12f1 
// ========================================================
// ========================================================
// 描 述：PointController.cs 
// 作 者：郑贤春 
// 时 间：2017/02/08 22:18:34 
// 版 本：5.4.1f1 
// ========================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.UIShape {

    public class UIBezierLinePointController {
        private float[] m_PointT;
        private Vector3[] m_PointNormal;
        private Vector3[] m_PointList;
        private float m_LineWidth;

        private Vector3[] m_LineUpPath;
        private Vector3[] m_LineDownPath;
        private bool m_MeshInitialized;

        /// <summary>
        /// 获取曲线上面的所有点
        /// </summary>
        /// <returns>The list.</returns>
        /// <param name="path">需要穿过的点列表</param>
        /// <param name="pointSize">两个点之间的节点数量</param>
        public Vector3[] PointList (List<Vector3> path, int pointSize) {
            Vector3[] controlPointList = PathControlPointGenerator (path);

            int smoothAmount = path.Count * pointSize;
            Vector3[] pointList = new Vector3[smoothAmount];

            for (int index = 1; index <= smoothAmount; index++) {
                Vector3 currPt = FindPointPosition (controlPointList, (float) index / smoothAmount);
                pointList[index - 1] = currPt;
            }
            return pointList;
        }

        /// <summary>
        /// 获取曲线上面的所有点
        /// </summary>
        public Vector3[] PointListAverage (List<Vector3> path, int allSize) {
            Vector3[] controlPointList = PathControlPointGenerator (path);

            Vector3[] pointList = new Vector3[allSize];
            float[] weight = pathWeight (path, allSize);
            for (int index = 0; index < allSize; index++) {
                if (index == 0) {
                    pointList[index] = path[0];
                }
                if (index == allSize - 1) {
                    pointList[index] = path[path.Count - 1];
                }
                Vector3 currPt = FindPointPosition (controlPointList, weight[index]);
                pointList[index] = currPt;
            }
            return pointList;
        }

        /// <summary>
        /// 获取曲线上面的所有点
        /// </summary>
        public Vector3[] PointListAverage (List<Vector3> path, float dx) {
            m_MeshInitialized = false;
            Vector3[] controlPointList = PathControlPointGenerator (path);

            List<Vector3> pointList = new List<Vector3> ();
            List<float> pointT = new List<float> ();
            List<Vector3> pointNormal = new List<Vector3> ();
            float deviation = 0.01f;
            float dt = 0.01f;
            float t = 0;

            Vector3 prePt = path[0];
            pointList.Add (prePt);
            pointT.Add (t);
            pointNormal.Add (Velocity (controlPointList, t));
            while (t + dt < 1) {
                Vector3 tempPt = FindPointPosition (controlPointList, t + dt);
                float dd = Vector3.Distance (prePt, tempPt);
                float dMax = 0f;
                float dMin = 0f;
                while (Math.Abs (dd - dx) > dx * deviation) {
                    if (dd > dx) {
                        dMax = dt;
                        dt = (dMax + dMin) / 2;
                    } else {
                        dMin = dt;
                        if (dMax != 0)
                            dt = (dMax + dMin) / 2;
                        else
                            dt = dt * 2;
                    }
                    tempPt = FindPointPosition (controlPointList, t + dt);
                    dd = Vector3.Distance (prePt, tempPt);
                }
                t = t + dt;
                prePt = tempPt;
                pointList.Add (prePt);
                pointT.Add (t);
                pointNormal.Add (Velocity (controlPointList, t));
            }
            pointList.Add (path[path.Count - 1]);
            pointT.Add (1);
            pointNormal.Add (Velocity (controlPointList, t));
            m_PointList = pointList.ToArray ();
            m_PointT = pointT.ToArray ();
            m_PointNormal = pointNormal.ToArray ();
            return pointList.ToArray ();
        }

        public Vector3[] GetPathUp (float lineWidth) {
            InitMeshPath (lineWidth);
            return m_LineUpPath;
        }

        public Vector3[] GetPathDown (float lineWidth) {
            InitMeshPath (lineWidth);
            return m_LineDownPath;
        }

        private void InitMeshPath (float lineWidth) {
            if (m_LineWidth != lineWidth) m_MeshInitialized = false;
            if (m_MeshInitialized) return;
            m_MeshInitialized = true;
            m_LineWidth = lineWidth;
            m_LineUpPath = new Vector3[m_PointList.Length];
            m_LineDownPath = new Vector3[m_PointList.Length];
            for (int i = 0; i < m_PointList.Length; i++) {
                Vector3 normal = m_PointNormal[i];
                Vector3 orthogonal = Vector3.zero;
                Vector3.OrthoNormalize (ref normal, ref orthogonal);
                m_LineUpPath[i] = m_PointList[i] + orthogonal * m_LineWidth;
                m_LineDownPath[i] = m_PointList[i] - orthogonal * m_LineWidth;
            }
        }

        private Vector3 GetNormal (Vector3[] path, Vector3 pt, float t) {
            float deviation = 0.00001f * Vector3.Distance (path[0], path[1]);
            float dt = 0.01f;
            Vector3 temp;
            if (t == 1) {
                temp = FindPointPosition (path, t - dt);
                while (Vector3.Distance (pt, temp) > deviation) {
                    dt = dt / 2;
                    temp = FindPointPosition (path, t - dt);
                }
                return pt - temp;
            }
            temp = FindPointPosition (path, t + dt);
            while (Vector3.Distance (pt, temp) > deviation) {
                dt = dt / 2;
                temp = FindPointPosition (path, t + dt);
            }
            return temp - pt;
        }

        public float[] pathWeight (List<Vector3> path, int allSize) {
            float distance = 0f;
            float[] distances = new float[path.Count];
            float[] dDistances = new float[path.Count];
            for (int i = 0; i < path.Count; i++) {
                float d = 0;
                if (i != 0)
                    d = Vector3.Distance (path[i - 1], path[i]);
                distance += d;
                distances[i] = distance;
                dDistances[i] = d;
            }
            float[] weight = new float[allSize];
            float averageD = distance / (allSize - 1);
            int index = 0;
            for (int i = 0; i < allSize; i++) {
                if (i == 0) {
                    weight[i] = 0;
                    continue;
                }
                if (i == allSize - 1) {
                    weight[i] = 1;
                    continue;
                }
                float d = i * averageD;
                while (d > distances[index] && index < distances.Length - 1) {
                    index++;
                }
                weight[i] = (float) (index - 1) / (path.Count - 1) + (d - distances[index - 1]) / (distances[index] - distances[index - 1]) / (path.Count - 1);
            }
            return weight;
        }

        /// <summary>
        /// 获取控制点
        /// </summary>
        public Vector3[] PathControlPointGenerator (List<Vector3> path) {
            int offset = 2;
            Vector3[] suppliedPath = path.ToArray();
            Vector3[] controlPoint = new Vector3[suppliedPath.Length + offset];
            Array.Copy (suppliedPath, 0, controlPoint, 1, suppliedPath.Length);

            controlPoint[0] = controlPoint[1] + (controlPoint[1] - controlPoint[2]);
            controlPoint[controlPoint.Length - 1] = controlPoint[controlPoint.Length - 2] + (controlPoint[controlPoint.Length - 2] - controlPoint[controlPoint.Length - 3]);

            if (controlPoint[1] == controlPoint[controlPoint.Length - 2]) {
                Vector3[] tmpLoopSpline = new Vector3[controlPoint.Length];
                Array.Copy (controlPoint, tmpLoopSpline, controlPoint.Length);
                tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
                tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
                controlPoint = new Vector3[tmpLoopSpline.Length];
                Array.Copy (tmpLoopSpline, controlPoint, tmpLoopSpline.Length);
            }

            return controlPoint;
        }

        /// <summary>
        /// 根据 T 获取曲线上面的点位置
        /// </summary>
        private Vector3 FindPointPosition (Vector3[] pts, float t) {
            int numSections = pts.Length - 3;
            int currPt = Mathf.Min (Mathf.FloorToInt (t * numSections), numSections - 1);

            Vector3 a = pts[currPt];
            Vector3 b = pts[currPt + 1];
            Vector3 c = pts[currPt + 2];
            Vector3 d = pts[currPt + 3];

            float u = t * numSections - currPt;
            return .5f * (
                (-a + 3f * b - 3f * c + d) * (u * u * u) +
                (2f * a - 5f * b + 4f * c - d) * (u * u) +
                (-a + c) * u +
                2f * b
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Velocity(Vector3[] pts, float t)
        {
            int numSections = pts.Length - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1);
            float u = t * numSections - currPt;

            Vector3 a = pts[currPt];
            Vector3 b = pts[currPt + 1];
            Vector3 c = pts[currPt + 2];
            Vector3 d = pts[currPt + 3];

            return 1.5f * (-a + 3f * b - 3f * c + d) * (u * u) +
                (2f * a - 5f * b + 4f * c - d) * u +
                .5f * c - .5f * a;
        }

        /// <summary>
        /// 根据 T 获取曲线上面的点切线
        /// </summary>
        private Vector3 FindPointTangent (Vector3[] pts, float t) {
            int numSections = pts.Length - 3;
            int currPt = Mathf.Min (Mathf.FloorToInt (t * numSections), numSections - 1);

            Vector3 p0 = pts[currPt];
            Vector3 p1 = pts[currPt + 1];
            Vector3 p2 = pts[currPt + 2];
            Vector3 p3 = pts[currPt + 3];

            float t2 = t * t;

            return 0.5f * (-p0 + p2) +
                (2.0f * p0 - 5.0f * p1 + 4 * p2 - p3) * t +
                (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t2 * 1.5f;
        }

    }

    

}