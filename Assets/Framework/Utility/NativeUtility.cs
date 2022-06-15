// ========================================================
// 描 述：NativeUtility.cs 
// 作 者： 
// 时 间：2019/06/30 12:01:28 
// 版 本：2018.3.12f1 
// ========================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Framework.Utility {
    public class NativeUtility {

        /// <summary>
        /// 打开 app 商城界面
        /// </summary>
        public static bool OpenStore (string packageName) {
#if UNITY_EDITOR
            
            return false;
#elif UNITY_ANDROID
            var storeUtilClass = new AndroidJavaClass("com.firststep.util.StoreUtil");
            var result = storeUtilClass.CallStatic<bool>(packageName);
            return result;
#elif UNITY_IHONE
            return false;
#else
            return false;
#endif
        }
    }
}