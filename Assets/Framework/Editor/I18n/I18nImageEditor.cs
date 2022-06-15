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
    [CustomEditor (typeof (I18nImage), true)]
    public class I18nImageEditor : Editor {

        private SerializedProperty m_idProp;
        private Transform transform;
        private Image m_image;

        private void OnEnable () {
            this.transform = (target as I18nImage).transform;
            m_image = transform.GetComponent<Image> ();
            m_idProp = serializedObject.FindProperty ("id");

            if (m_image != null && m_image.sprite != null) {
                m_idProp.stringValue = m_image.sprite.name;
                serializedObject.ApplyModifiedProperties ();
            }
        }

        public override void OnInspectorGUI () {
            base.OnInspectorGUI ();

            serializedObject.ApplyModifiedProperties ();
        }
    }
}