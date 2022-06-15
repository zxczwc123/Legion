// ========================================================
// 描 述：ScreenCaptureEditor.cs 
// 作 者： 
// 时 间：2020/03/19 09:52:23 
// 版 本：2019.2.1f1 
// ========================================================
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using LitJson;
using System;
using System.Xml.Serialization;
using System.Xml;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;

namespace Framework.MEditor
{

    public class ScreenCaptureEditor{

        [MenuItem("Framework/Tool/Capture", false, 0)]
        public static void Capture()
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var filename = EditorUtility.SaveFilePanel("保存文件", dir, "", "png");
            ScreenCapture.CaptureScreenshot(filename,0);
        }

    }
}

