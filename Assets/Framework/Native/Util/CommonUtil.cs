using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Native.Util {
    public class CommonUtil {

        /// <summary>
        /// 获取后临时复制存储
        /// </summary>
        public static string DeviceId;

        /// <summary>
        /// 打开qq聊天
        /// </summary>
        /// <param name="qq"></param>
        public static void OpenQQChat(string qq) {
#if !UNITY_EDITOR && UNITY_ANDROID
            var commonUtilClass = new AndroidJavaClass("com.unity3d.util.CommonUtil");
            commonUtilClass.CallStatic("openQQChat", qq);
#elif !UNITY_EDITOR && UNITY_IOS
            CommonExternUtil.openQQChat(qq);
#else
            Debug.Log("打开qq聊天 :" + qq);
#endif
        }

        public static void CopyTextToClipBoard(string data) {
#if !UNITY_EDITOR && UNITY_ANDROID
            var commonUtilClass = new AndroidJavaClass("com.unity3d.util.CommonUtil");
            commonUtilClass.CallStatic("copyTextToClipboard", data);
#elif !UNITY_EDITOR && UNITY_IOS
            CommonExternUtil.copyTextToClipboard(data);
#else
            Debug.Log("复制到剪切板 :" + data);
#endif
        }

        public static void GetDeviceId(Action<string> callback) {
            if (DeviceId != null) {
                if (callback != null) callback(DeviceId);
                return;
            }
#if !UNITY_EDITOR && UNITY_ANDROID
            var commonUtilClass = new AndroidJavaClass("com.unity3d.util.CommonUtil");
            var listener = new IGetDeviceIdListener();
            listener.SetListener(callback);
            commonUtilClass.CallStatic("getDeviceId",listener);
#elif !UNITY_EDITOR && UNITY_IOS
            CommonUtil.DeviceId = CommonExternUtil.getDeviceId();
            if (callback != null) callback(CommonUtil.DeviceId);
#else
            Debug.Log("GetDeviceId");
            if (callback != null) callback("");
#endif
        }

        public static string GetDeviceModel() {
#if !UNITY_EDITOR && UNITY_ANDROID
            var commonUtilClass = new AndroidJavaClass("com.unity3d.util.CommonUtil");
            return commonUtilClass.CallStatic<string>("getDeviceModel");
#elif !UNITY_EDITOR && UNITY_IOS
            return UnityEngine.SystemInfo.deviceModel;
            //return CommonExternUtil.getDeviceModel();
#else
            Debug.Log("getDeviceModel");
            return "";
#endif
        }

        public static string GetMD5FromAndroidStreaming(string filename) {
            var commonUtilClass = new AndroidJavaClass("com.unity3d.util.CommonUtil");
            return commonUtilClass.CallStatic<string>("getMD5FromAssets",filename);
        }

    }

    public class IGetDeviceIdListener : AndroidJavaProxy {

        private Action<string> _callback;

        public IGetDeviceIdListener(): base("com.unity3d.util.IGetDeviceIdListener") {

        }

        public void SetListener(Action<string> callback) {
            this._callback = callback;
        }

        public void onDeviceIdCallback(string deviceId) {
            CommonUtil.DeviceId = deviceId;
            if (_callback != null) _callback(deviceId);
        }
    }

#if UNITY_EDITOR || UNITY_IOS
    public class CommonExternUtil {
        [DllImport("__Internal")]
        public static extern void openQQChat(string qq);

        [DllImport("__Internal")]
        public static extern void copyTextToClipboard(string data);

        [DllImport("__Internal")]
        public static extern string getDeviceId();

        [DllImport("__Internal")]
        public static extern string getDeviceModel();
    }
#endif

}