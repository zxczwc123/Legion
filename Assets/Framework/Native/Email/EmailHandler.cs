// ========================================================
// 描 述：EmailHandler.cs 
// 作 者： 
// 时 间：2020/01/07 17:33:23 
// 版 本：2019.2.1f1 
// ========================================================
using System;
#if !UNITY_EDITOR && UNITY_IPHONE
using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace Framework.Email
{

    public class EmailHandler {

        /// <summary>
        /// 打开邮件发送
        /// </summary>
        /// <param name="uri"></param>
        public static void OpenEmail (string uri) {
#if UNITY_EDITOR

#elif UNITY_IPHONE
            openEmail(uri);
#elif UNITY_ANDROID
            string EmailHandleClassName = "com.firststep.email.EmailHandler";
            AndroidJavaClass emailClass = new AndroidJavaClass (EmailHandleClassName);
            emailClass.CallStatic ("openEmail",uri);
#else

#endif
        }

#if !UNITY_EDITOR && UNITY_IPHONE
        [DllImport("__Internal")]
        private static extern void openEmail(string uri);
#endif

    }
}