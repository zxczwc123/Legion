using UnityEngine;
using System.Collections;
using System.IO;
using Framework.MEditor;

public class ChangeScriptTemplates : UnityEditor.AssetModificationProcessor
{

    // 添加脚本注释模板
    private static string annotation = "// ========================================================\r\n";
    private static string str = 
    annotation +
    "// 描 述：#ScriptName# \r\n" + 
    "// 创 建：#CoderName# \r\n" + 
    "// 时 间：#CreateTime# \r\n" +
    "// 版 本：#UnityVersion# \r\n" +
    annotation;

    // 创建资源调用
    public static void OnWillCreateAsset(string path)
    {
        // 只修改C#脚本
        path = path.Replace(".meta", "");
        if (path.EndsWith(".cs"))
        {
            // 只有在Framework 和 Game目录下的文件才添加
            if(!path.Contains("Assets/FrameWork") && !path.Contains("Assets/Game"))
            {
                return;
            }
            string allText = "";
            var fileText = File.ReadAllText(path);
            if (!fileText.Contains(annotation))
            {
                allText += str;
            }
            allText += fileText;
            // 替换作者
            allText = allText.Replace("#ScriptName#", Path.GetFileName(path));
            // 替换作者
            allText = allText.Replace("#CoderName#", Setting.GetDeveloperName());
            // 替换字符串为系统时间
            allText = allText.Replace("#CreateTime#", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            // 替换版本信息
            allText = allText.Replace("#UnityVersion#", Application.unityVersion);
            string content = File.ReadAllText(path);
            if (content.Contains(annotation)) return;
            File.WriteAllText(path, allText);
        }
    }
}
