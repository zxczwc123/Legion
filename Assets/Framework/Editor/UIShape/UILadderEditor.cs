// ========================================================
// 描 述：UILadderEditor.cs 
// 作 者：郑贤春 
// 时 间：2017/03/01 08:53:29 
// 版 本：5.4.1f1 
// ========================================================
using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Framework.UIShape {
[CustomEditor(typeof(UILadder))]
public class UILadderEditor : UnityEditor.UI.ImageEditor
{
    private SerializedProperty m_propertyWidth;
    private SerializedProperty m_propertyDivider;

    protected override void OnEnable()
    {
        m_propertyWidth = serializedObject.FindProperty("m_width");
        m_propertyDivider = serializedObject.FindProperty("m_divider");
        base.OnEnable();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_propertyWidth);
        EditorGUILayout.PropertyField(m_propertyDivider);
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}
}