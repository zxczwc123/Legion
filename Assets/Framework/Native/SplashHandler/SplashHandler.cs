// ========================================================
// 描 述：SplashHandler.cs 
// 作 者： 
// 时 间：2019/06/29 09:17:36 
// 版 本：2018.3.12f1 
// ========================================================
#if UNITY_IPHONE && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace Framework.Splash {
    public class SplashHandler {

        /// <summary>
        /// 游戏加载结束隐藏
        /// </summary>
        public static void HideSplash () {
#if UNITY_EDITOR
#elif UNITY_IPHONE
            hideSplash();
#elif UNITY_ANDROID
            string splashHandleClassName = "com.unity3d.splash.SplashHandler";
            AndroidJavaClass splashHandlerClass = new AndroidJavaClass (splashHandleClassName);
            splashHandlerClass.CallStatic ("hideSplash");
#else
#endif

        }

#if UNITY_IPHONE && !UNITY_EDITOR
        [DllImport ("__Internal")]
        internal static extern void hideSplash ();
#endif
    }
}