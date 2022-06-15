using Framework.Core;
using System;
using System.Runtime.InteropServices;

#if UNITY_EDITOR || UNITY_IOS
namespace Framework.Native.WeChat {

    public class WeChatMgrIosClient : IWeChatMgrClient {

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WeChatPayCallback(int code, string orderId);
        [AOT.MonoPInvokeCallback(typeof(WeChatPayCallback))]
        static void onWeChatPayCallback(int code, string orderId) {
            instance.onPayCallback(code, orderId);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WeChatLoginCallback(string code);
        [AOT.MonoPInvokeCallback(typeof(WeChatLoginCallback))]
        static void onWeChatLoginCallback(string code) {
            instance.onLoginCallback(code);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WeChatShareCallback(int code);
        [AOT.MonoPInvokeCallback(typeof(WeChatShareCallback))]
        static void onWeChatShareCallback(int code) {
            instance.onShareCallback(code);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WeChatMiniProgramCallback(string extraData);
        [AOT.MonoPInvokeCallback(typeof(WeChatMiniProgramCallback))]
        static void onWeChatMiniProgramCallback(string extraData) {
            instance.onMiniProgramCallback(extraData);
        }

        private static WeChatMgrIosClient instance;

        private WeChatMgr _wechatMgr;

        public WeChatMgrIosClient(WeChatMgr weChatMgr) {
            instance = this;

            _wechatMgr = weChatMgr;

            WeChatPayCallback payCallback = new WeChatPayCallback(onWeChatPayCallback);
            IntPtr fpPayCallback = Marshal.GetFunctionPointerForDelegate(payCallback);
            WeChatExtern.setWeChatPayCallback(fpPayCallback);

            WeChatLoginCallback loginCallback = new WeChatLoginCallback(onWeChatLoginCallback);
            IntPtr fpLoginCallback = Marshal.GetFunctionPointerForDelegate(loginCallback);
            WeChatExtern.setWeChatLoginCallback(fpLoginCallback);

            WeChatShareCallback shareCallback = new WeChatShareCallback(onWeChatShareCallback);
            IntPtr fpShareCallback = Marshal.GetFunctionPointerForDelegate(shareCallback);
            WeChatExtern.setWeChatShareCallback(fpShareCallback);

            WeChatMiniProgramCallback minProgramCallback = new WeChatMiniProgramCallback(onWeChatMiniProgramCallback);
            IntPtr fpMiniProgramCallback = Marshal.GetFunctionPointerForDelegate(minProgramCallback);
            WeChatExtern.setWeChatMiniProgramCallback(fpMiniProgramCallback);
        }

        /// <summary>
        /// 加载初始化
        /// </summary>
        public void Load() {
            WeChatExtern.initWeChat();
        }

        /// <summary>
        /// 卸载
        /// </summary>
        public void Unload() {

        }

        /// <summary>
        /// 原生监听回调
        /// </summary>
        /// <param name="code"></param>
        public void onLoginCallback(string code) {
            FrameworkEngine.Instance.RunOnMain(() => {
                this._wechatMgr.OnNativeLoginCallback(code);
            });
        }

        /// <summary>
        /// 原生监听回调
        /// </summary>
        /// <param name="code"></param>
        public void onShareCallback(int code) {
            FrameworkEngine.Instance.RunOnMain(() => {
                this._wechatMgr.OnNativeShareCallback(code);
            });

        }

        public void onPayCallback(int code, string orderId) {
            FrameworkEngine.Instance.RunOnMain(() => {
                this._wechatMgr.OnNativePayCallback(code, orderId);
            });
        }

        public void onMiniProgramCallback(string extraData) {
            FrameworkEngine.Instance.RunOnMain(() => {
                this._wechatMgr.OnWeChatMiniCallback(extraData);
            });
        }

        /// <summary>
        /// 微信是否安装
        /// </summary>
        /// <returns></returns>
        public bool IsWXAppInstalled() {
            return WeChatExtern.isWXAppInstalled();
        }

        /// <summary>
        /// 发起微信授权登陆
        /// </summary>
        public void Login() {
            WeChatExtern.loginWeChat();
        }

        /// <summary>
        /// 分享图片
        /// </summary>
        /// <param name="title"></param>
        /// <param name="imagePath"></param>
        /// <param name="description"></param>
        /// <param name="scene"></param>
        public void ShareImage(string title, string imagePath, string description, int scene) {
            WeChatExtern.shareImage(title, imagePath, description, scene);
        }

        /// <summary>
        /// 分享链接
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="description"></param>
        /// <param name="scene"></param>
        public void ShareUrl(string title, string url, string description, int scene) {
            WeChatExtern.shareUrl(title, url, description, scene);
        }

        /// <summary>
        /// 分享文本
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="scene"></param>
        public void ShareText(string title, string text, int scene) {
            WeChatExtern.shareText(title, text, scene);
        }

        
        public void Pay(string appid, string partnerid, string prepayid, string noncestr, string timestamp, string pack, string sign) {
            WeChatExtern.pay( partnerid,  prepayid,  noncestr,  timestamp,  pack,  sign);
        }

        public void LaunchMiniProgram(string username, string path) {
            WeChatExtern.launchMiniProgram(username, path);
        }
    }

    public class WeChatExtern {
        [DllImport("__Internal")]
        internal static extern void setWeChatPayCallback(IntPtr callback);
        [DllImport("__Internal")]
        internal static extern void setWeChatLoginCallback(IntPtr callback);
        [DllImport("__Internal")]
        internal static extern void setWeChatShareCallback(IntPtr callback);
        [DllImport("__Internal")]
        internal static extern void setWeChatMiniProgramCallback(IntPtr callback);


        [DllImport("__Internal")]
        internal static extern void initWeChat();

        [DllImport("__Internal")]
        internal static extern bool isWXAppInstalled();

        [DllImport("__Internal")]
        internal static extern void loginWeChat();

        [DllImport("__Internal")]
        internal static extern void shareText(string title, string text, int scene);

        [DllImport("__Internal")]
        internal static extern void shareUrl(string title, string url, string description, int scene);

        [DllImport("__Internal")]
        internal static extern void shareImage(string title, string imagePath, string description, int scene);

        [DllImport("__Internal")]
        internal static extern void pay(string partnerid, string prepayid, string noncestr, string timestamp, string pack, string sign);

        [DllImport("__Internal")]
        internal static extern void launchMiniProgram(string username, string path);
    }

}
#endif