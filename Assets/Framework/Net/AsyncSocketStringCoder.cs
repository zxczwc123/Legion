// ========================================================
// 描 述：AsyncSocketStringCoder.cs 
// 作 者：郑贤春 
// 时 间：2019/06/15 15:57:23 
// 版 本：2018.3.12f1 
// ========================================================
using UnityEngine;
using System.Collections;
using System.Text;

namespace Framework.Net
{
    public class AsyncSocketStringCoder : AsyncSocketDataCoder<string>
    {
        public string Decode(byte[] bytes)
        {
            string data = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Debug.Log(string.Format("收到服务器消息:{0}", data));
            return data;
        }

        public  byte[] Encode(string obj)
        {
            string data = obj as string;
            return System.Text.Encoding.UTF8.GetBytes(data);
        }
    }
}

