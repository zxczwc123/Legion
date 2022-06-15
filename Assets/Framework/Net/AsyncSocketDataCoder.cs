// ========================================================
// 描 述：AsyncSocketDataCoder.cs 
// 作 者：郑贤春 
// 时 间：2019/06/15 15:57:23 
// 版 本：2018.3.12f1 
// ========================================================
// ========================================================
// 描 述：AsyncSocketClientCoder.cs 
// 作 者：郑贤春 
// 时 间：2019/06/15 15:46:11 
// 版 本：2018.3.12f1 
// ========================================================
using UnityEngine;
using System.Collections;
using System;
using System.IO;

namespace Framework.Net
{
    public interface AsyncSocketDataCoder<T>
    {
        T Decode(byte[] bytes);

        byte[] Encode(T obj);
    }
    
}

