// ========================================================
// 描 述：SettingEditor.cs 
// 作 者： 
// 时 间：2017/01/02 16:49:28 
// 版 本：5.4.1f1 
// ========================================================
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Framework.MEditor
{
    public class SettingWindow : EditorWindow
    {
        [MenuItem("Framework/Settings", false,-11)]
        public static void ShowSettingWindow()
        {
            SettingWindow.ShowWindow();
        }
        
        public static void ShowWindow()
        {
            SettingWindow window = EditorWindow.GetWindow<SettingWindow>(true, "Settings");
            window.maxSize = new Vector2(320, 400);
            window.minSize = new Vector2(320, 400);
        }

        string m_developerName;

        void OnEnable()
        {
            this.m_developerName = Setting.GetDeveloperName();
        }

        void OnGUI()
        {
            m_developerName = EditorGUILayout.TextField("作者名:", m_developerName);
            if(GUILayout.Button("保存",GUILayout.Height(17)))
            {
                Setting.SetDeveloperName(m_developerName);
            }
        }
    }

    public class Setting
    {

        public const string DEVELOPER_NAME = "Game.Developer";

        public static string GetDeveloperName()
        {
            return EditorPrefs.GetString(DEVELOPER_NAME);
        }

        public static void SetDeveloperName(string name)
        {
            EditorPrefs.SetString(DEVELOPER_NAME, name);
        }
    }
}

