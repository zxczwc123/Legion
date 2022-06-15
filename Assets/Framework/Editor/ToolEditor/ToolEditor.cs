// ========================================================
// 描 述：ToolEditor.cs 
// 作 者：郑贤春 
// 时 间：2017/02/08 10:20:51 
// 版 本：5.4.1f1 
// ========================================================
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Reflection;
using System;

namespace Framework.MEditor
{
    public class ToolEditor : EditorWindow
    {
        [MenuItem("Framework/Tool/ClearPlayerPrefs", false, 0)]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

    }
}


