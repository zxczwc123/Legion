// ========================================================
// 描 述：UIStandardBezierLinePointController.cs 
// 作 者： 
// 时 间：2019/07/21 22:14:23 
// 版 本：2018.3.12f1 
// ========================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.UIShape {

    public class UIStandardBezierLinePointController {
        public Vector3[] pathPositions {
            get;
            private set;
        }
        public Vector3[] normalList {
            get;
            private set;
        }
        public Vector3[] upPathPositions {
            get;
            private set;
        }
        public Vector3[] downPathPositions {
            get;
            private set;
        }

        /// <summary>
        /// 获取曲线上面的所有点
        /// </summary>
        /// <returns>The list.</returns>
        /// <param name="path">需要穿过的点列表</param>
        /// <param name="pointSize">两个点之间的节点数量 包括两端点</param>
        public void UpdatePath (List<Vector3> path, int pointSize) {
            this.pathPositions = new Vector3[pointSize];
            this.normalList = new Vector3[pointSize];
            for (int index = 0; index < pointSize; index++) {
                float t = index / (float) (pointSize - 1);
                Vector3 currPt = CalculateBezierPoint (t, path);
                this.pathPositions[index] = currPt;
                var currNormal = CalculateBezierNormal(t,path);
                this.normalList[index] = currNormal;
            }
        }

        public void UpdateMeshPath (float lineWidth) {
            this.upPathPositions = new Vector3[pathPositions.Length];
            this.downPathPositions = new Vector3[pathPositions.Length];
            for (int i = 0; i < this.pathPositions.Length; i++) {
                Vector3 normal = this.normalList[i];
                Vector3 ortho = Vector3.zero;
                Vector3.OrthoNormalize (ref normal, ref ortho);
                this.upPathPositions[i] = this.pathPositions[i] + ortho * lineWidth;
                this.downPathPositions[i] = this.pathPositions[i] - ortho * lineWidth;
            }
        }

        private Vector3 CalculateBezierNormal (float t, List<Vector3> path) {
            if (path.Count == 4) {
                var p0 = path[0];
                var p1 = path[1];
                var p2 = path[2];
                var p3 = path[3];
                return CalculateBezierNormal (t, p0, p1, p2, p3);
            } else {
                var p0 = path[0];
                var p1 = path[1];
                var p2 = path[2];
                return CalculateBezierNormal (t, p0, p1, p2);
            }
        }

        private Vector3 CalculateBezierNormal (float t, Vector3 p0, Vector3 p1, Vector3 p2) {
            Vector3 p = (t - 1) * p0 + (1 - 2 * t) * p1 + t * p2;
            return p;
        }

        private Vector3 CalculateBezierNormal (float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            Vector3 p = (t - 1) * p0 + (1 - 2 * t) * p1 + 3 * t * p2 + t * p3;
            return p;
        }

        private Vector3 CalculateBezierPoint (float t, List<Vector3> path) {
            if (path.Count == 4) {
                var p0 = path[0];
                var p1 = path[1];
                var p2 = path[2];
                var p3 = path[3];
                return CalculateBezierPoint (t, p0, p1, p2, p3);
            } else {
                var p0 = path[0];
                var p1 = path[1];
                var p2 = path[2];
                return CalculateBezierPoint (t, p0, p1, p2);
            }
        }

        private Vector3 CalculateBezierPoint (float t, Vector3 p0, Vector3 p1, Vector3 p2) {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            Vector3 p = uu * p0 + 2 * u * t * p1 + tt * p2;
            return p;
        }

        private Vector3 CalculateBezierPoint (float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            float u = 1 - t;
            float tt = t * t;
            float ttt = t * t * t;
            float uu = u * u;
            float uuu = u * u * u;
            Vector3 p = uuu * p0 + 3 * t * uu * p1 + 3 * tt * u * p2 + ttt * p3;
            return p;
        }

    }
}