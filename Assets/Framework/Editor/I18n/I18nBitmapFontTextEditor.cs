/*
 * OnionEngine Framework for Unity By wzl
 * -------------------------------------------------------------------
 * Name    : I18nText
 * Date    : 2018/10/31
 * Version : v1.0
 * Describe: 
 */

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.I18n
{
    [CustomEditor (typeof (I18nBitmapFontText), true)]
    public class I18nBitmapFontTextEditor : Editor {

        private SerializedProperty m_idProp;
        private Transform transform;
        private Text m_text;

        private void OnEnable () {
            this.transform = (target as I18nBitmapFontText).transform;
            m_text = transform.GetComponent<Text> ();
            m_idProp = serializedObject.FindProperty ("id");

            if (m_text != null && m_text.font != null) {
                m_idProp.stringValue = m_text.font.name;
                serializedObject.ApplyModifiedProperties ();
            }
        }

        public override void OnInspectorGUI () {
            base.OnInspectorGUI ();

            serializedObject.ApplyModifiedProperties ();
        }
    }
}