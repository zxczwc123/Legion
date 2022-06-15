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
    [CustomEditor (typeof (I18nText), true)]
    public class I18nTextEditor : Editor {

        private Transform transform;

        private SerializedProperty m_idProp;
        
        private Text m_text;

        private void OnEnable () {
            this.transform = (target as I18nText).transform;
            m_text = transform.GetComponent<Text> ();
            m_idProp = serializedObject.FindProperty ("id");

            if (!string.IsNullOrEmpty (m_text.text)) {
                m_idProp.stringValue = m_text.text;
                serializedObject.ApplyModifiedProperties ();
            }
        }

        public override void OnInspectorGUI () {
            base.OnInspectorGUI ();

            serializedObject.ApplyModifiedProperties ();
        }
    }
}