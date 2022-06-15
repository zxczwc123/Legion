using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Native.WeChat {
    public enum SendMessageToWXScene : int {
        WXSceneSession = 0,
        WXSceneTimeline = 1,
        WXSceneFavorite = 2,
        WXSceneSpecifiedContact = 3
    }

    public class WeChatMgr {

        private static WeChatMgr _instance;

        public static WeChatMgr Instance {
            get {
                if (WeChatMgr._instance == null) {
                    WeChatMgr._instance = new WeChatMgr();
                }
                return WeChatMgr._instance;
            }

        }

        private IWeChatMgrClient _wechatMgrClient;

        public Action<string> OnWeChatLoginCallback;

        public Action<int> OnWeChatShareCallback;

        public Action<int, string> OnWeChatPayCallback;

        public Action<string> OnWeChatMiniCallback;

        private WeChatMgr() {
#if UNITY_EDITOR || UNITY_STANDALONE
            this._wechatMgrClient = new WeChatMgrWinClient(this);
#elif UNITY_IOS
            this._wechatMgrClient = new WeChatMgrIosClient(this);
#else
            this._wechatMgrClient = new WeChatMgrAndClient(this);
#endif
        }

        /// <summary>
        /// 原生监听回调
        /// </summary>
        /// <param name="code"></param>
        public void OnNativeLoginCallback(string code) {
            if (this.OnWeChatLoginCallback != null) this.OnWeChatLoginCallback(code);
        }

        /// <summary>
        /// 原生监听回调
        /// </summary>
        /// <param name="code"></param>
        public void OnNativeShareCallback(int code) {
            if (this.OnWeChatShareCallback != null) this.OnWeChatShareCallback(code);
        }

        public void OnNativePayCallback(int code,string orderId) {
            if (this.OnWeChatPayCallback != null) this.OnWeChatPayCallback(code,orderId);
        }

        public void OnNativePayCallback(string extraData) {
            if (this.OnWeChatMiniCallback != null) this.OnWeChatMiniCallback( extraData);
        }

        /// <summary>
        /// 加载初始化
        /// </summary>
        public void Load() {
            this._wechatMgrClient.Load();
        }

        /// <summary>
        /// 卸载
        /// </summary>
        public void Unload() {
            this._wechatMgrClient.Unload();
        }

        /// <summary>
        /// 微信是否安装
        /// </summary>
        /// <returns></returns>
        public bool IsWXAppInstalled() {
            return this._wechatMgrClient.IsWXAppInstalled();
        }

        /// <summary>
        /// 发起微信授权登陆
        /// </summary>
        public void Login() {
            this._wechatMgrClient.Login();
        }

        /// <summary>
        /// 分享图片
        /// </summary>
        /// <param name="title"></param>
        /// <param name="imagePath"></param>
        /// <param name="description"></param>
        /// <param name="scene"></param>
        public void ShareImage(string title, string imagePath, string description, int scene = (int)SendMessageToWXScene.WXSceneSession) {
            this._wechatMgrClient.ShareImage(title, imagePath, description, scene);
        }

        /// <summary>
        /// 分享链接
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="description"></param>
        /// <param name="scene"></param>
        public void ShareUrl(string title, string url, string description, int scene = (int)SendMessageToWXScene.WXSceneSession) {
            this._wechatMgrClient.ShareUrl(title, url, description, scene);
        }

        /// <summary>
        /// 分享文本
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="scene"></param>
        public void ShareText(string title, string text, int scene = (int)SendMessageToWXScene.WXSceneSession) {
            this._wechatMgrClient.ShareText(title, text, scene);
        }

        public void Pay(string appid, string partnerid, string prepayid, string noncestr, string timestamp, string pack, string sign) {
            this._wechatMgrClient.Pay(appid, partnerid, prepayid, noncestr, timestamp, pack, sign);
        }

        public void LaunchMiniProgram(string username, string path) {
            this._wechatMgrClient.LaunchMiniProgram(username, path);
        }
    }

    /// <summary>
    /// 支付实例
    /// </summary>
    public interface IWeChatMgrClient {
        /// <summary>
        /// 加载初始化
        /// </summary>
        void Load();

        /// <summary>
        /// 卸载
        /// </summary>
        void Unload();

        /// <summary>
        /// 微信是否安装
        /// </summary>
        /// <returns></returns>
        bool IsWXAppInstalled();

        /// <summary>
        /// 发起微信授权登陆
        /// </summary>
        void Login();

        /// <summary>
        /// 分享文本
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="scene"></param>
        void ShareText(string title, string text, int scene);

        /// <summary>
        /// 分享链接
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="description"></param>
        /// <param name="scene"></param>
        void ShareUrl(string title, string url, string description, int scene);

        /// <summary>
        /// 分享图片
        /// </summary>
        /// <param name="title"></param>
        /// <param name="imagePath"></param>
        /// <param name="description"></param>
        /// <param name="scene"></param>
        void ShareImage(string title, string imagePath, string description, int scene);

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="nonceStr"></param>
        /// <param name="pack"></param>
        /// <param name="signType"></param>
        /// <param name="paySign"></param>
        void Pay(string appid, string partnerid, string prepayid, string noncestr, string timestamp, string pack, string sign);

        void LaunchMiniProgram(string username, string path);
    }
   

}
