// ========================================================
// 描 述：ModuleGenerateWindow.cs 
// 作 者：郑贤春 
// 时 间：2017/05/14 12:31:34 
// 版 本：5.5.2f1 
// ========================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Framework.MEditor
{
    public class ModuleGenerateWindow : EditorWindow
    {
        public static void ShowWindow()
        {
            ModuleGenerateWindow window = EditorWindow.GetWindow<ModuleGenerateWindow>(true, "ModuleGenerator");
            window.maxSize = new Vector2(340, 400);
            window.minSize = new Vector2(340, 400);
        }

        public string moduleRootPath
        {
            get
            {
                return Application.dataPath + "/Module";
            }
        }

        private string m_moduleName = "";

        private void OnEnable()
        {

        }

        private void OnGUI()
        {
            this.m_moduleName = EditorGUILayout.TextField("模块名称：", this.m_moduleName);
            if (GUILayout.Button("生成"))
            {
                if (string.IsNullOrEmpty(this.m_moduleName))
                {
                    EditorUtility.DisplayDialog("提示", "模块名称不能为空！", "确定");
                }
                else
                {
                    if (Contains(this.m_moduleName))
                    {
                        EditorUtility.DisplayDialog("提示", "模块已存在！", "确定");
                    }
                    else
                    {
                        GenerateModule(this.m_moduleName);
                    }
                }
            }
        }

        private bool Contains(string moduleName)
        {
            string path = Path.Combine(this.moduleRootPath, moduleName);
            return Directory.Exists(path);
        }

        private void GenerateModule(string moduleName)
        {
            string path = Path.Combine(this.moduleRootPath, moduleName);
            Directory.CreateDirectory(path);
            string filename = Path.Combine(path, moduleName + ".cs");
            ModuleScriptGenerator generator = new ModuleScriptGenerator(moduleName);
            using (StreamWriter sw = new StreamWriter(new FileStream(filename, FileMode.OpenOrCreate)))
            {
                sw.WriteLine(generator.ToString());
            }
            AssetDatabase.Refresh();
        }

    }
}
