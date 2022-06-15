// ========================================================
// 描 述：UIStandardBezierLineEditor.cs 
// 作 者： 
// 时 间：2019/07/21 22:14:23 
// 版 本：2018.3.12f1 
// ========================================================
// ========================================================
// 描 述：UIBezierLineEditor.cs 
// 作 者： 
// 时 间：2019/07/19 15:12:25 
// 版 本：2018.3.12f1 
// ========================================================
// ========================================================
// 描 述：BezierLine.cs 
// 作 者：郑贤春 
// 时 间：2017/02/10 19:46:48 
// 版 本：5.4.1f1 
// ========================================================
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.UIShape {

    [CustomEditor (typeof (UIStandardBezierLine))]
    public class UIStandardBezierLineEditor : Editor {
        private UIStandardBezierLine m_blineRenderer;
        private List<Vector3> m_pathList;

        float m_arrowSize = 1;

        void OnEnable () {
            this.m_blineRenderer = target as UIStandardBezierLine;
            this.m_blineRenderer.Init ();
            this.m_pathList = this.m_blineRenderer.GetPath ();
        }

        void OnDisable () {

        }

        public override void OnInspectorGUI () {
            
            this.m_blineRenderer.UpdateMesh ();
            base.OnInspectorGUI ();
        }

        void OnSceneGUI () {
            for (int i = 0; this.m_pathList != null && i < this.m_pathList.Count; i++) {
                EditorGUI.BeginChangeCheck ();
                Vector3 position = Handles.PositionHandle (this.m_blineRenderer.transform.localToWorldMatrix.MultiplyPoint (this.m_pathList[i]), Quaternion.identity);
                if (EditorGUI.EndChangeCheck ()) {
                    this.m_pathList[i] = this.m_blineRenderer.transform.worldToLocalMatrix.MultiplyPoint (position);
                    this.m_blineRenderer.SetPath (this.m_pathList);
                    EditorUtility.SetDirty(m_blineRenderer);
                    m_blineRenderer.SetAllDirty();
                }
            }
        }

        void OnSceneFunc (SceneView sceneView) {

        }
    }

    [CustomEditor (typeof (UIStandardBezierLine2))]
    public class UIStandardBezierLine2Editor : Editor {
        private UIStandardBezierLine2 m_blineRenderer;
        private List<Vector3> m_pathList;

        float m_arrowSize = 1;

        void OnEnable () {
            this.m_blineRenderer = target as UIStandardBezierLine2;
            this.m_blineRenderer.Init ();
            this.m_pathList = this.m_blineRenderer.GetPath ();
        }

        void OnDisable () {

        }

        public override void OnInspectorGUI () {
            
            this.m_blineRenderer.UpdateMesh ();
            base.OnInspectorGUI ();
        }

        void OnSceneGUI () {
            for (int i = 0; this.m_pathList != null && i < this.m_pathList.Count; i++) {
                EditorGUI.BeginChangeCheck ();
                Vector3 position = Handles.PositionHandle (this.m_blineRenderer.transform.localToWorldMatrix.MultiplyPoint (this.m_pathList[i]), Quaternion.identity);
                if (EditorGUI.EndChangeCheck ()) {
                    this.m_pathList[i] = this.m_blineRenderer.transform.worldToLocalMatrix.MultiplyPoint (position);
                    this.m_blineRenderer.SetPath (this.m_pathList);
                    EditorUtility.SetDirty(m_blineRenderer);
                    m_blineRenderer.SetAllDirty();
                }
            }
        }

        void OnSceneFunc (SceneView sceneView) {

        }
    }

}