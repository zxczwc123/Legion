// ========================================================
// 描 述：TxtUtility.cs 
// 作 者：郑贤春 
// 时 间：2017/02/18 09:59:47 
// 版 本：5.4.1f1 
// ========================================================
using System.IO;
using UnityEditor;
using System;

namespace Framework.MEditor.Utility
{
    public class TxtUtility
    {
        /// <summary>
        /// 保存到文件
        /// </summary>
        public static void StringToFile(string content, string filename = null)
        {
            if (string.IsNullOrEmpty(filename))
            {
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                filename = EditorUtility.SaveFilePanel("保存文件", dir, "", "txt");
            }
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("filename Can not be null or Empty!");
            }
            if(!filename.EndsWith(".txt"))
            {
                filename = filename + ".txt";
            }
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(content);
                fs.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// 获取文件内容
        /// </summary>
        public static string FileToString(string filename)
        {
            if (string.IsNullOrEmpty(filename)) throw new ArgumentException("Filename can't be null or empty!");
            if (!File.Exists(filename)) throw new ArgumentException("File not exist!");
            using (StreamReader sr = new StreamReader(new FileStream(filename,FileMode.Open)))
            {
                string line = "";
                string result = "";
                while((line = sr.ReadLine()) != null)
                {
                    result += line;
                    result += "\n";
                }
                if (result.Length < 1) return string.Empty;
                return result.Substring(0,result.Length -1);
            }
        }
    }
}

