// ========================================================
// 描 述：UITriangleEditor.cs 
// 作 者：郑贤春 
// 时 间：2017/03/03 21:30:36 
// 版 本：5.4.1f1 
// ========================================================
using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Framework.UIShape {

[CustomEditor(typeof(UITriangle))]
public class UITriangleEditor : UnityEditor.UI.ImageEditor
{
    private SerializedProperty m_propertySize;

    protected override void OnEnable()
    {
        m_propertySize = serializedObject.FindProperty("m_size");
        base.OnEnable();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_propertySize);
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }

}
}
