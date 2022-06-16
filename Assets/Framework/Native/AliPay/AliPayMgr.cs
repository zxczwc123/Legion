using Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Native.AliPay {

    public class AliPayMgr {

        private static AliPayMgr _instance;

        public static AliPayMgr Instance {
            get {
                if (AliPayMgr._instance == null) {
                    AliPayMgr._instance = new AliPayMgr();
                }
                return AliPayMgr._instance;
            }

        }

        private IAliPayMgrClient _alipayMgrClient;

        public Action<int, string> OnAliPayPayCallback;

        public Action<int, string> OnAliPayAuthCallback;

        public AliPayMgr() {
#if UNITY_EDITOR || UNITY_STANDALONE
            this._alipayMgrClient = new AliPayMgrWinClient(this);
#elif UNITY_IOS
            this._alipayMgrClient = new AliPayMgrIosClient(this);
#else
            this._alipayMgrClient = new AliPayMgrAndClient(this);
#endif
        }

        /// <summary>
        /// 原生监听回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="result"></param>
        public void OnPayCallback(int code, string result) {
            if (this.OnAliPayPayCallback != null) this.OnAliPayPayCallback(code, result);
        }

        /// <summary>
        /// 原生监听回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="result"></param>
        public void OnAuthCallback(int code, string result) {
            if (this.OnAliPayAuthCallback != null) this.OnAliPayAuthCallback(code, result);
        }

        /// <summary>
        /// 加载初始化
        /// </summary>
        public void Load() {
            this._alipayMgrClient.Load();
        }

        /// <summary>
        /// 卸载
        /// </summary>
        public void UnLoad() {
            this._alipayMgrClient.UnLoad();
        }

        /// <summary>
        ///  是否安装
        /// </summary>
        /// <returns></returns>
        public bool IsAliAppInstalled() {
            return this._alipayMgrClient.IsAliAppInstalled();
        }

        /// <summary>
        /// 发起支付
        /// </summary>
        /// <param name="orderInfo"></param>
        public void Pay(string orderInfo) {
            this._alipayMgrClient.Pay(orderInfo);
        }

    }

#if UNITY_EDITOR || UNITY_ANDROID

    public class AliPayMgrAndClient : AndroidJavaProxy, IAliPayMgrClient {

        private static string androidAliPayListenerClass = "com.unity3d.alipay.IAliPayListener";


        private AliPayMgr _alipayMgr;

        private AndroidJavaClass aliPayClass;

        public AliPayMgrAndClient(AliPayMgr aliPayMgr):base(androidAliPayListenerClass) {

            this._alipayMgr = aliPayMgr;

            aliPayClass = new AndroidJavaClass("com.unity3d.alipay.AliPay");

            aliPayClass.CallStatic("setListener", this);
        }

        /// <summary>
        /// 加载初始化
        /// </summary>
        public void Load() {
            aliPayClass.CallStatic("init");
        }

        /// <summary>
        /// 卸载
        /// </summary>
        public void UnLoad() {
            aliPayClass.CallStatic("setListener", null);
        }

        /// <summary>
        /// 原生监听回调* code 1 成功 0 失败
        /// </summary>
        /// <param name="code"></param>
        /// <param name="result"></param>
        public void onPayCallback(int code, string result) {
            Engine.instance.RunOnMain(() => {
                this._alipayMgr.OnPayCallback(code, result);
            });
        }

        /// <summary>
        /// 原生监听回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="result"></param>
        public void onAuthCallback(int code, string result) {
            Engine.instance.RunOnMain(() => {
                this._alipayMgr.OnAuthCallback(code, result);
            });
        }


        /// <summary>
        /// 是否安装
        /// </summary>
        /// <returns></returns>
        public bool IsAliAppInstalled() {
            return aliPayClass.CallStatic<bool>("isAliAppInstalled");

        }

        /// <summary>
        /// 发起支付
        /// </summary>
        /// <param name="orderInfo"></param>
        public void Pay(string orderInfo) {
            aliPayClass.CallStatic("pay", orderInfo);
        }
    }
#endif

#if UNITY_EDITOR || UNITY_IOS
    public class AliPayMgrIosClient : IAliPayMgrClient {

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void AliPayCallback( int code,string result);
        [AOT.MonoPInvokeCallback(typeof(AliPayCallback))]
        static void onAliPayCallback(int code,string result) {
            instance.onPayCallback(code, result);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void AliAuthCallback(int code, string result);
        [AOT.MonoPInvokeCallback(typeof(AliAuthCallback))]
        static void onAliAuthCallback(int code, string result) {
            instance.onAuthCallback(code, result);
        }

        private static AliPayMgrIosClient instance;

        private AliPayMgr _alipayMgr;


        public AliPayMgrIosClient(AliPayMgr aliPayMgr) {
            instance = this;
            this._alipayMgr = aliPayMgr;

            AliPayCallback payCallback = new AliPayCallback(onAliPayCallback);
            IntPtr fpPayCallback = Marshal.GetFunctionPointerForDelegate(payCallback);
            AliPayExtern.setAliPayCallback(fpPayCallback);

            AliAuthCallback authCallback = new AliAuthCallback(onAliAuthCallback);
            IntPtr fpAuthCallback = Marshal.GetFunctionPointerForDelegate(authCallback);
            AliPayExtern.setAliAuthCallback(fpAuthCallback);
        }

        /// <summary>
        /// 加载初始化
        /// </summary>
        public void Load() {
            AliPayExtern.initAli();
        }

        /// <summary>
        /// 卸载
        /// </summary>
        public void UnLoad() {

        }

        /// <summary>
        /// 原生监听回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="result"></param>
        public void onPayCallback(int code, string result) {
            Engine.instance.RunOnMain(() => {
                this._alipayMgr.OnAliPayPayCallback(code, result);
            });
        }

        /// <summary>
        /// 原生监听回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="result"></param>
        public void onAuthCallback(int code, string result) {
            Engine.instance.RunOnMain(() => {
                this._alipayMgr.OnAuthCallback(code, result);
            });
        }

        /// <summary>
        /// 微信是否安装
        /// </summary>
        /// <returns></returns>
        public bool IsAliAppInstalled() {
            return AliPayExtern.isAliAppInstalled();
        }

        /// <summary>
        /// 发起支付
        /// </summary>
        /// <param name="orderInfo"></param>
        public void Pay(string orderInfo) {
            AliPayExtern.payAli(orderInfo);
        }

    }

    public class AliPayExtern {

        [DllImport("__Internal")]
        internal static extern void setAliPayCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setAliAuthCallback(IntPtr callback);


        [DllImport("__Internal")]
        internal static extern void initAli();

        [DllImport("__Internal")]
        internal static extern bool isAliAppInstalled();

        [DllImport("__Internal")]
        internal static extern void payAli(string orderInfo);

    }
#endif

  
    public class AliPayMgrWinClient : IAliPayMgrClient {

        public AliPayMgrWinClient(AliPayMgr aliPayMgr) {

        }

        /// <summary>
        /// 加载初始化
        /// </summary>
        public void Load() {

        }

        public void UnLoad() {

        }

        /// <summary>
        /// 原生监听回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="result"></param>
        public void onPayCallback(int code, string result) {

        }

        /// <summary>
        /// 原生监听回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="result"></param>
        public void onAuthCallback(int code, string result) {
            Debug.Log("ali auth editor");
        }

        /// <summary>
        /// 是否安装
        /// </summary>
        /// <returns></returns>
        public bool IsAliAppInstalled() {
            return false;
        }

        /// <summary>
        /// 发起支付
        /// </summary>
        /// <param name="orderInfo"></param>
        public void Pay(string orderInfo) {
            Debug.Log("ali pay editor");
        }

    }

    /// <summary>
    /// 支付实例
    /// </summary>
    public interface IAliPayMgrClient {
        /// <summary>
        /// 加载初始化
        /// </summary>
        void Load();

        /// <summary>
        /// 卸载
        /// </summary>
        void UnLoad();

        /// <summary>
        /// 原生监听回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="result"></param>
        void onPayCallback(int code, string result);

        /// <summary>
        /// 原生监听回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="result"></param>
        void onAuthCallback(int code, string result);

        /// <summary>
        /// 是否安装
        /// </summary>
        /// <returns></returns>
        bool IsAliAppInstalled();

        /// <summary>
        /// 发起支付
        /// </summary>
        /// <param name="orderInfo"></param>
        void Pay(string orderInfo);

    }

}
