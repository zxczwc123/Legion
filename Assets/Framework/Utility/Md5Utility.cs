// ========================================================
// 描 述：Md5Utility.cs 
// 作 者： 
// 时 间：2020/07/23 23:05:04 
// 版 本：2019.2.1f1 
// ========================================================

using System.IO;
using System.Text;
using UnityEngine;

namespace Framework.Utility
{
    public class MD5Utility
    {
        public static string GetMD5HashFromFile(string filename)
        {
            try
            {
#if UNITY_IOSa
                FileStream file = File.OpenRead(filename);
#else
                FileStream file = new FileStream(filename, FileMode.Open);
#endif
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (System.Exception ex)
            {
                Debug.Log("错误信息：" + ex);
                return null;
            }
        }
    }
}