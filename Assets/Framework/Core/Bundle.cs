// ========================================================
// 描 述：Bundle.cs 
// 作 者：郑贤春 
// 时 间：2017/01/02 17:43:35 
// 版 本：5.4.1f1 
// ========================================================
using System;
using System.Collections.Generic;

namespace Framework.Core
{
    /// <summary>
    /// 模块传递数据
    /// </summary>
    public class Bundle
    {
        public string stringValue;
        public int intValue;
        public bool boolValue;
        public object objectValue;
        public Action callback;
        public Dictionary<string, string> stringDict;
        public Dictionary<string, int> intDict;
        public Dictionary<string, object> objectDict;
        public Dictionary<string, bool> boolDict;
        public Dictionary<string, Action> callbackDict;

        public Bundle(){
            intDict = new Dictionary<string, int>();
            objectDict = new Dictionary<string, object>();
            callbackDict = new Dictionary<string, Action>();
            stringDict = new Dictionary<string, string>();
            boolDict = new Dictionary<string, bool>();
        }

    }
}

