using Framework.Core;
using System;
using UnityEngine;

#if UNITY_EDITOR || UNITY_ANDROID
namespace Framework.Native.WeChat {

    public interface IAndroidProxyListenter {
        /// <summary>
        /// 原生监听回调
        /// </summary>
        /// <param name="code"></param>
        void onLoginCallback(string code);

        /// <summary>
        /// 原生监听回调
        /// </summary>
        /// <param name="code"></param>
        void onShareCallback(int code);

        /// <summary>
        /// 支付回调
        /// </summary>
        /// <param name="code"></param>
        void onPayCallback(int code, string orderId);

        /// <summary>
        /// 小程序会回调
        /// </summary>
        /// <param name="extraData"></param>
        void onMiniProgramCallback(string extraData);
    }


    public class WeChatMgrAndClient :  IWeChatMgrClient {

        public class WeChatProxyListener : AndroidJavaProxy {

            private static string androidWeChatListenerClass = "com.unity3d.wechat.IWeChatListener";


            private WeChatMgrAndClient client;

            public WeChatProxyListener(WeChatMgrAndClient client) : base(androidWeChatListenerClass) {
                this.client = client;
            }

            /// <summary>
            /// 原生监听回调
            /// </summary>
            /// <param name="code"></param>
            private void onLoginCallback(string code) {
                this.client.onLoginCallback(code);
            }

            /// <summary>
            /// 原生监听回调
            /// </summary>
            /// <param name="code"></param>
            private void onShareCallback(int code) {
                this.client.onShareCallback(code);

            }

            private void onPayCallback(int code, string orderId) {
                this.client.onPayCallback(code,orderId);
            }

            private void onMiniProgramCallback(string extraData) {
                this.client.onMiniProgramCallback(extraData);
            }
        }


        private WeChatMgr _wechatMgr;

        private AndroidJavaClass wechatClass;

        private WeChatProxyListener weChatProxyListener;
        

        public WeChatMgrAndClient(WeChatMgr weChatMgr) {

            
            this._wechatMgr = weChatMgr;

            this.weChatProxyListener = new WeChatProxyListener(this);

            wechatClass = new AndroidJavaClass("com.unity3d.wechat.WeChat");

            wechatClass.CallStatic("setListener", this.weChatProxyListener);

        }

        /// <summary>
        /// 加载初始化
        /// </summary>
        public void Load() {
            wechatClass.CallStatic("init");
        }

        /// <summary>
        /// 卸载
        /// </summary>
        public void Unload() {
            wechatClass.CallStatic("setListener", null);
            Debug.Log("WeChatMgrAndClient Unload");
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

        public void onPayCallback(int code,string orderId) {
            FrameworkEngine.Instance.RunOnMain(() => {
                this._wechatMgr.OnNativePayCallback(code,orderId);
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
            return wechatClass.CallStatic<bool>("isWXAppInstalled");

        }

        /// <summary>
        /// 发起微信授权登陆
        /// </summary>
        public void Login() {
            wechatClass.CallStatic("login");
        }

        /// <summary>
        /// 分享图片
        /// </summary>
        /// <param name="title"></param>
        /// <param name="imagePath"></param>
        /// <param name="description"></param>
        /// <param name="scene"></param>
        public void ShareImage(string title, string imagePath, string description, int scene) {
            wechatClass.CallStatic("shareImage", title, imagePath, description, scene);
        }

        /// <summary>
        /// 分享链接
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="description"></param>
        /// <param name="scene"></param>
        public void ShareUrl(string title, string url, string description, int scene) {
            wechatClass.CallStatic("shareUrl", title, url, description, scene);
        }

        /// <summary>
        /// 分享文本
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="scene"></param>
        public void ShareText(string title, string text, int scene) {
            wechatClass.CallStatic("shareText", title, text, scene);
        }

        public void Pay(string appId,string partnerid, string prepayid, string noncestr, string timestamp, string pack, string sign) { 
            wechatClass.CallStatic("pay",appId, partnerid, prepayid, noncestr, timestamp, pack, sign);
        }

        public void LaunchMiniProgram(string username, string path) {
            wechatClass.CallStatic("launchMiniProgram", username, path);
        }
    }

}
#endif