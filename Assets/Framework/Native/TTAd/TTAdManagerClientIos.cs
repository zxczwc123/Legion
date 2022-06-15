using Framework.Core;
using System;
using System.Runtime.InteropServices;

namespace Framework.Native.TTAd {
    public class TTAdManagerClientIos : TTAdManager {

        static TTAdManagerClientIos ttAdManagerIosClient;

        // Banner
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void BannerAdViewDidLoadCallback();
        [AOT.MonoPInvokeCallback(typeof(BannerAdViewDidLoadCallback))]
        static void onBannerAdViewDidLoadCallback() {
            ttAdManagerIosClient.OnBannerAdViewDidLoadCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void BannerAdViewDidLoadFailCallback(int code,string msg);
        [AOT.MonoPInvokeCallback(typeof(BannerAdViewDidLoadFailCallback))]
        static void onBannerAdViewDidLoadFailCallback(int code, string msg) {
            ttAdManagerIosClient.OnBannerAdViewDidLoadFailCallback( code,msg);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void BannerAdViewRenderSuccessCallback();
        [AOT.MonoPInvokeCallback(typeof(BannerAdViewRenderSuccessCallback))]
        static void onBannerAdViewRenderSuccessCallback() {
            ttAdManagerIosClient.OnBannerAdViewRenderSuccessCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void BannerAdViewRenderFailCallback(int code, string msg);
        [AOT.MonoPInvokeCallback(typeof(BannerAdViewRenderFailCallback))]
        static void onBannerAdViewRenderFailCallback(int code, string msg) {
            ttAdManagerIosClient.OnBannerAdViewRenderFailCallback(code, msg);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void BannerAdViewWillBecomVisibleCallback();
        [AOT.MonoPInvokeCallback(typeof(BannerAdViewWillBecomVisibleCallback))]
        static void onBannerAdViewWillBecomVisibleCallback() {
            ttAdManagerIosClient.OnBannerAdViewWillBecomVisibleCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void BannerAdViewDidClickCallback();
        [AOT.MonoPInvokeCallback(typeof(BannerAdViewDidClickCallback))]
        static void onBannerAdViewDidClickCallback() {
            ttAdManagerIosClient.OnBannerAdViewDidClickCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void BannerAdViewDislikeWithReasonCallback();
        [AOT.MonoPInvokeCallback(typeof(BannerAdViewDislikeWithReasonCallback))]
        static void onBannerAdViewDislikeWithReasonCallback() {
            ttAdManagerIosClient.OnBannerAdViewDislikeWithReasonCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void BannerAdViewDidCloseOtherControllerCallback();
        [AOT.MonoPInvokeCallback(typeof(BannerAdViewDidCloseOtherControllerCallback))]
        static void onBannerAdViewDidCloseOtherControllerCallback() {
            ttAdManagerIosClient.OnBannerAdViewDidCloseOtherControllerCallback();
        }

        // reward video
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RewardVideoDidLoadCallback();
        [AOT.MonoPInvokeCallback(typeof(RewardVideoDidLoadCallback))]
        static void onRewardVideoDidLoadCallback() {
            ttAdManagerIosClient.OnRewardVideoDidLoadCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RewardVideoDidLoadFailCallback( int code,string msg);
        [AOT.MonoPInvokeCallback(typeof(RewardVideoDidLoadFailCallback))]
        static void onRewardVideoDidLoadFailCallback(int code,string msg) {
            ttAdManagerIosClient.OnRewardVideoDidLoadFailCallback(code, msg);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RewardVideoRenderSuccessCallback();
        [AOT.MonoPInvokeCallback(typeof(RewardVideoRenderSuccessCallback))]
        static void onRewardVideoRenderSuccessCallback() {
            ttAdManagerIosClient.OnRewardVideoRenderSuccessCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RewardVideoRenderFailCallback(int code, string msg);
        [AOT.MonoPInvokeCallback(typeof(RewardVideoRenderFailCallback))]
        static void onRewardVideoRenderFailCallback(int code, string msg) {
            ttAdManagerIosClient.OnRewardVideoRenderFailCallback(code, msg);
        }


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RewardVideoDidVisibleCallback();
        [AOT.MonoPInvokeCallback(typeof(RewardVideoDidVisibleCallback))]
        static void onRewardVideoDidVisibleCallback() {
            ttAdManagerIosClient.OnRewardVideoDidVisibleCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RewardVideoDidCloseCallback();
        [AOT.MonoPInvokeCallback(typeof(RewardVideoDidCloseCallback))]
        static void onRewardVideoDidCloseCallback() {
            ttAdManagerIosClient.OnRewardVideoDidCloseCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RewardVideoDidClickCallback();
        [AOT.MonoPInvokeCallback(typeof(RewardVideoDidClickCallback))]
        static void onRewardVideoDidClickCallback() {
            ttAdManagerIosClient.OnRewardVideoDidClickCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RewardVideoDidClickSkipCallback();
        [AOT.MonoPInvokeCallback(typeof(RewardVideoDidClickSkipCallback))]
        static void onRewardVideoDidClickSkipCallback() {
            ttAdManagerIosClient.OnRewardVideoDidClickSkipCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RewardVideoServerRewardDidSuccessCallback();
        [AOT.MonoPInvokeCallback(typeof(RewardVideoServerRewardDidSuccessCallback))]
        static void onRewardVideoServerRewardDidSuccessCallback() {
            ttAdManagerIosClient.OnRewardVideoServerRewardDidSuccessCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RewardVideoServerRewardDidFailCallback(int code, string msg);
        [AOT.MonoPInvokeCallback(typeof(RewardVideoServerRewardDidFailCallback))]
        static void onRewardVideoServerRewardDidFailCallback(int code, string msg) {
            ttAdManagerIosClient.OnRewardVideoServerRewardDidFailCallback(code, msg);
        }

        // full video
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FullVideoDidLoadCallback();
        [AOT.MonoPInvokeCallback(typeof(FullVideoDidLoadCallback))]
        static void onFullVideoDidLoadCallback() {
            ttAdManagerIosClient.OnFullVideoDidLoadCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FullVideoDidLoadFailCallback( int code,string msg);
        [AOT.MonoPInvokeCallback(typeof(FullVideoDidLoadFailCallback))]
        static void onFullVideoDidLoadFailCallback(int code,string msg) {
            ttAdManagerIosClient.OnFullVideoDidLoadFailCallback(code, msg);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FullVideoRenderSuccessCallback();
        [AOT.MonoPInvokeCallback(typeof(FullVideoRenderSuccessCallback))]
        static void onFullVideoRenderSuccessCallback() {
            ttAdManagerIosClient.OnFullVideoRenderSuccessCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FullVideoRenderFailCallback(int code, string msg);
        [AOT.MonoPInvokeCallback(typeof(FullVideoRenderFailCallback))]
        static void onFullVideoRenderFailCallback(int code, string msg) {
            ttAdManagerIosClient.OnFullVideoRenderFailCallback(code, msg);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FullVideoDidVisibleCallback();
        [AOT.MonoPInvokeCallback(typeof(FullVideoDidVisibleCallback))]
        static void onFullVideoDidVisibleCallback() {
            ttAdManagerIosClient.OnFullVideoDidVisibleCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FullVideoDidCloseCallback();
        [AOT.MonoPInvokeCallback(typeof(FullVideoDidCloseCallback))]
        static void onFullVideoDidCloseCallback() {
            ttAdManagerIosClient.OnFullVideoDidCloseCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FullVideoDidClickCallback();
        [AOT.MonoPInvokeCallback(typeof(FullVideoDidClickCallback))]
        static void onFullVideoDidClickCallback() {
            ttAdManagerIosClient.OnFullVideoDidClickCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FullVideoDidClickSkipCallback();
        [AOT.MonoPInvokeCallback(typeof(FullVideoDidClickSkipCallback))]
        static void onFullVideoDidClickSkipCallback() {
            ttAdManagerIosClient.OnFullVideoDidClickSkipCallback();
        }

        // interstitial
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void InterstitialDidLoadCallback();
        [AOT.MonoPInvokeCallback(typeof(InterstitialDidLoadCallback))]
        static void onInterstitialDidLoadCallback() {
            ttAdManagerIosClient.OnInterstitialDidLoadCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void InterstitialDidLoadFailCallback( int code,string msg);
        [AOT.MonoPInvokeCallback(typeof(InterstitialDidLoadFailCallback))]
        static void onInterstitialDidLoadFailCallback( int code,string msg) {
            ttAdManagerIosClient.OnInterstitialDidLoadFailCallback(code, msg);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void InterstitialRenderSuccessCallback();
        [AOT.MonoPInvokeCallback(typeof(InterstitialRenderSuccessCallback))]
        static void onInterstitialRenderSuccessCallback() {
            ttAdManagerIosClient.OnInterstitialRenderSuccessCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void InterstitialRenderFailCallback(int code, string msg);
        [AOT.MonoPInvokeCallback(typeof(InterstitialRenderFailCallback))]
        static void onInterstitialRenderFailCallback(int code, string msg) {
            ttAdManagerIosClient.OnInterstitialRenderFailCallback(code, msg);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void InterstitialWillVisibleCallback();
        [AOT.MonoPInvokeCallback(typeof(InterstitialWillVisibleCallback))]
        static void onInterstitialWillVisibleCallback() {
            ttAdManagerIosClient.OnInterstitialWillVisibleCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void InterstitialDidCloseCallback();
        [AOT.MonoPInvokeCallback(typeof(InterstitialDidCloseCallback))]
        static void onInterstitialDidCloseCallback() {
            ttAdManagerIosClient.OnInterstitialDidCloseCallback();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void InterstitialDidClickCallback();
        [AOT.MonoPInvokeCallback(typeof(InterstitialDidClickCallback))]
        static void onInterstitialDidClickCallback() {
            ttAdManagerIosClient.OnInterstitialDidClickCallback();
        }

        public override void Init(string appId) {
            Externs.init(appId);

            ttAdManagerIosClient = this;

            // banner
            BannerAdViewDidLoadCallback bannerAdViewDidLoadCallback = new BannerAdViewDidLoadCallback(onBannerAdViewDidLoadCallback);
            IntPtr fpBannerAdViewDidLoad = Marshal.GetFunctionPointerForDelegate(bannerAdViewDidLoadCallback);
            Externs.setBannerAdViewDidLoadCallback(fpBannerAdViewDidLoad);

            BannerAdViewDidLoadFailCallback bannerAdViewDidLoadFailCallback = new BannerAdViewDidLoadFailCallback(onBannerAdViewDidLoadFailCallback);
            IntPtr fpBannerAdViewDidLoadFail = Marshal.GetFunctionPointerForDelegate(bannerAdViewDidLoadFailCallback);
            Externs.setBannerAdViewDidLoadFailCallback(fpBannerAdViewDidLoadFail);

            BannerAdViewRenderSuccessCallback bannerAdViewRenderSuccessCallback = new BannerAdViewRenderSuccessCallback(onBannerAdViewRenderSuccessCallback);
            IntPtr fpBannerAdViewRenderSuccess = Marshal.GetFunctionPointerForDelegate(bannerAdViewRenderSuccessCallback);
            Externs.setBannerAdViewRenderSuccessCallback(fpBannerAdViewRenderSuccess);

            BannerAdViewRenderFailCallback bannerAdViewRenderFailCallback = new BannerAdViewRenderFailCallback(onBannerAdViewRenderFailCallback);
            IntPtr fpBannerAdViewRenderFail = Marshal.GetFunctionPointerForDelegate(bannerAdViewRenderFailCallback);
            Externs.setBannerAdViewRenderFailCallback(fpBannerAdViewRenderFail);

            BannerAdViewWillBecomVisibleCallback bannerAdViewWillBecomVisibleCallback = new BannerAdViewWillBecomVisibleCallback(onBannerAdViewWillBecomVisibleCallback);
            IntPtr fpBannerAdViewWillBecomVisible = Marshal.GetFunctionPointerForDelegate(bannerAdViewWillBecomVisibleCallback);
            Externs.setBannerAdViewWillBecomVisibleCallback(fpBannerAdViewWillBecomVisible);

            BannerAdViewDidClickCallback bannerAdViewDidClickCallback = new BannerAdViewDidClickCallback(onBannerAdViewDidClickCallback);
            IntPtr fpBannerAdViewDidClick = Marshal.GetFunctionPointerForDelegate(bannerAdViewDidLoadFailCallback);
            Externs.setBannerAdViewDidClickCallback(fpBannerAdViewDidClick);

            BannerAdViewDislikeWithReasonCallback bannerAdViewDislikeWithReasonCallback = new BannerAdViewDislikeWithReasonCallback(onBannerAdViewDislikeWithReasonCallback);
            IntPtr fpBannerAdViewDislikeWithReason = Marshal.GetFunctionPointerForDelegate(bannerAdViewDislikeWithReasonCallback);
            Externs.setBannerAdViewDislikeWithReasonCallback(fpBannerAdViewDislikeWithReason);

            BannerAdViewDidCloseOtherControllerCallback bannerAdViewDidCloseOtherControllerCallback = new BannerAdViewDidCloseOtherControllerCallback(onBannerAdViewDidCloseOtherControllerCallback);
            IntPtr fpBannerAdViewDidCloseOtherController = Marshal.GetFunctionPointerForDelegate(bannerAdViewDidCloseOtherControllerCallback);
            Externs.setBannerAdViewDidCloseOtherControllerCallback(fpBannerAdViewDidCloseOtherController);

            // rewardvideo
            RewardVideoDidLoadCallback rewardVideoDidLoadCallback = new RewardVideoDidLoadCallback(onRewardVideoDidLoadCallback);
            IntPtr fpRewardVideoDidLoad = Marshal.GetFunctionPointerForDelegate(rewardVideoDidLoadCallback);
            Externs.setRewardVideoDidLoadCallback(fpRewardVideoDidLoad);

            RewardVideoDidLoadFailCallback rewardVideoDidLoadFailCallback = new RewardVideoDidLoadFailCallback(onRewardVideoDidLoadFailCallback);
            IntPtr fpRewardVideoDidLoadFail = Marshal.GetFunctionPointerForDelegate(rewardVideoDidLoadFailCallback);
            Externs.setRewardVideoDidLoadFailCallback(fpRewardVideoDidLoadFail);

            RewardVideoRenderSuccessCallback rewardVideoRenderSuccessCallback = new RewardVideoRenderSuccessCallback(onRewardVideoRenderSuccessCallback);
            IntPtr fpRewardVideoRenderSuccess = Marshal.GetFunctionPointerForDelegate(rewardVideoRenderSuccessCallback);
            Externs.setRewardVideoRenderSuccessCallback(fpRewardVideoRenderSuccess);

            RewardVideoRenderFailCallback rewardVideoRenderFailCallback = new RewardVideoRenderFailCallback(onRewardVideoRenderFailCallback);
            IntPtr fpRewardVideoRenderFail = Marshal.GetFunctionPointerForDelegate(rewardVideoRenderFailCallback);
            Externs.setRewardVideoRenderFailCallback(fpRewardVideoRenderFail);

            RewardVideoDidCloseCallback rewardVideoDidCloseCallback = new RewardVideoDidCloseCallback(onRewardVideoDidCloseCallback);
            IntPtr fpRewardVideoDidClose = Marshal.GetFunctionPointerForDelegate(rewardVideoDidCloseCallback);
            Externs.setRewardVideoDidCloseCallback(fpRewardVideoDidClose);

            RewardVideoDidVisibleCallback rewardVideoDidVisibleCallback = new RewardVideoDidVisibleCallback(onRewardVideoDidVisibleCallback);
            IntPtr fpRewardVideoDidVisible = Marshal.GetFunctionPointerForDelegate(rewardVideoDidVisibleCallback);
            Externs.setRewardVideoDidVisibleCallback(fpRewardVideoDidVisible);

            RewardVideoDidClickCallback rewardVideoDidClickCallback = new RewardVideoDidClickCallback(onRewardVideoDidClickCallback);
            IntPtr fpRewardVideoDidClick = Marshal.GetFunctionPointerForDelegate(rewardVideoDidClickCallback);
            Externs.setRewardVideoDidClickCallback(fpRewardVideoDidClick);

            RewardVideoDidClickSkipCallback rewardVideoDidClickSkipCallback = new RewardVideoDidClickSkipCallback(onRewardVideoDidClickSkipCallback);
            IntPtr fpRewardVideoDidClickSkip = Marshal.GetFunctionPointerForDelegate(rewardVideoDidClickSkipCallback);
            Externs.setRewardVideoDidClickSkipCallback(fpRewardVideoDidClickSkip);

            RewardVideoServerRewardDidSuccessCallback rewardVideoServerRewardDidSuccessCallback = new RewardVideoServerRewardDidSuccessCallback(onRewardVideoServerRewardDidSuccessCallback);
            IntPtr fpRewardVideoServerRewardDidSuccess = Marshal.GetFunctionPointerForDelegate(rewardVideoServerRewardDidSuccessCallback);
            Externs.setRewardVideoServerRewardDidSuccessCallback(fpRewardVideoServerRewardDidSuccess);

            RewardVideoServerRewardDidFailCallback rewardVideoServerRewardDidFailCallback = new RewardVideoServerRewardDidFailCallback(onRewardVideoServerRewardDidFailCallback);
            IntPtr fpRewardVideoServerRewardDidFail = Marshal.GetFunctionPointerForDelegate(rewardVideoServerRewardDidFailCallback);
            Externs.setRewardVideoServerRewardDidFailCallback(fpRewardVideoServerRewardDidFail);

            // fullVideo
            FullVideoDidLoadCallback fullVideoDidLoadCallback = new FullVideoDidLoadCallback(onFullVideoDidLoadCallback);
            IntPtr fpFullVideoDidLoad = Marshal.GetFunctionPointerForDelegate(fullVideoDidLoadCallback);
            Externs.setFullVideoDidLoadCallback(fpFullVideoDidLoad);

            FullVideoDidLoadFailCallback fullVideoDidLoadFailCallback = new FullVideoDidLoadFailCallback(onFullVideoDidLoadFailCallback);
            IntPtr fpFullVideoDidLoadFail = Marshal.GetFunctionPointerForDelegate(fullVideoDidLoadFailCallback);
            Externs.setFullVideoDidLoadFailCallback(fpFullVideoDidLoadFail);

            FullVideoRenderSuccessCallback fullVideoRenderSuccessCallback = new FullVideoRenderSuccessCallback(onFullVideoRenderSuccessCallback);
            IntPtr fpFullVideoRenderSuccess = Marshal.GetFunctionPointerForDelegate(fullVideoRenderSuccessCallback);
            Externs.setFullVideoRenderSuccessCallback(fpFullVideoRenderSuccess);

            FullVideoRenderFailCallback fullVideoRenderFailCallback = new FullVideoRenderFailCallback(onFullVideoRenderFailCallback);
            IntPtr fpFullVideoRenderFail = Marshal.GetFunctionPointerForDelegate(fullVideoRenderFailCallback);
            Externs.setFullVideoRenderFailCallback(fpFullVideoRenderFail);

            FullVideoDidVisibleCallback fullVideoDidVisibleCallback = new FullVideoDidVisibleCallback(onFullVideoDidVisibleCallback);
            IntPtr fpFullVideoDidVisible = Marshal.GetFunctionPointerForDelegate(fullVideoDidVisibleCallback);
            Externs.setFullVideoDidVisibleCallback(fpFullVideoDidVisible);

            FullVideoDidCloseCallback fullVideoDidCloseCallback = new FullVideoDidCloseCallback(onFullVideoDidCloseCallback);
            IntPtr fpFullVideoDidClose = Marshal.GetFunctionPointerForDelegate(fullVideoDidCloseCallback);
            Externs.setFullVideoDidCloseCallback(fpFullVideoDidClose);

            FullVideoDidClickCallback fullVideoDidClickCallback = new FullVideoDidClickCallback(onFullVideoDidClickCallback);
            IntPtr fpFullVideoDidClick = Marshal.GetFunctionPointerForDelegate(fullVideoDidClickCallback);
            Externs.setFullVideoDidClickCallback(fpFullVideoDidClick);

            FullVideoDidClickSkipCallback fullVideoDidClickSkipCallback = new FullVideoDidClickSkipCallback(onFullVideoDidClickSkipCallback);
            IntPtr fpFullVideoDidClickSkip = Marshal.GetFunctionPointerForDelegate(fullVideoDidClickSkipCallback);
            Externs.setFullVideoDidClickSkipCallback(fpFullVideoDidClickSkip);

            // interstitial
            InterstitialDidLoadCallback interstitialDidLoadCallback = new InterstitialDidLoadCallback(onInterstitialDidLoadCallback);
            IntPtr fpInterstitialDidLoad = Marshal.GetFunctionPointerForDelegate(interstitialDidLoadCallback);
            Externs.setInterstitialDidLoadCallback(fpInterstitialDidLoad);

            InterstitialDidLoadFailCallback interstitialDidLoadFailCallback = new InterstitialDidLoadFailCallback(onInterstitialDidLoadFailCallback);
            IntPtr fpInterstitialDidLoadFail = Marshal.GetFunctionPointerForDelegate(interstitialDidLoadFailCallback);
            Externs.setInterstitialDidLoadFailCallback(fpInterstitialDidLoadFail);

            InterstitialRenderSuccessCallback interstitialRenderSuccessCallback = new InterstitialRenderSuccessCallback(onInterstitialRenderSuccessCallback);
            IntPtr fpInterstitialRenderSuccess = Marshal.GetFunctionPointerForDelegate(interstitialRenderSuccessCallback);
            Externs.setInterstitialRenderSuccessCallback(fpInterstitialRenderSuccess);

            InterstitialRenderFailCallback interstitialRenderFailCallback = new InterstitialRenderFailCallback(onInterstitialRenderFailCallback);
            IntPtr fpInterstitialRenderFail = Marshal.GetFunctionPointerForDelegate(interstitialRenderFailCallback);
            Externs.setInterstitialRenderFailCallback(fpInterstitialRenderFail);

            InterstitialWillVisibleCallback interstitialWillVisibleCallback = new InterstitialWillVisibleCallback(onInterstitialWillVisibleCallback);
            IntPtr fpInterstitialWillVisible = Marshal.GetFunctionPointerForDelegate(interstitialWillVisibleCallback);
            Externs.setInterstitialWillVisibleCallback(fpInterstitialWillVisible);

            InterstitialDidCloseCallback interstitialDidCloseCallback = new InterstitialDidCloseCallback(onInterstitialDidCloseCallback);
            IntPtr fpInterstitialDidClose = Marshal.GetFunctionPointerForDelegate(interstitialDidCloseCallback);
            Externs.setInterstitialDidCloseCallback(fpInterstitialDidClose);

            InterstitialDidClickCallback interstitialDidClickCallback = new InterstitialDidClickCallback(onInterstitialDidClickCallback);
            IntPtr fpInterstitialDidClick = Marshal.GetFunctionPointerForDelegate(interstitialDidClickCallback);
            Externs.setInterstitialDidClickCallback(fpInterstitialDidClick);
        }

        public override void Unload() {
            
        }

        // banner

        public override void LoadAdBanner(string codeId) {
            Externs.loadAdBanner(codeId);
        }

        public override void ShowAdBanner() {
            Externs.showAdBanner();
        }

        private void OnBannerAdViewDidLoadCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnBannerAdLoad != null) ttAdManagerIosClient.OnBannerAdLoad(BANNER_LOADED);
            });
        }

        private void OnBannerAdViewDidLoadFailCallback( int code,string msg) {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnBannerAdAction != null) ttAdManagerIosClient.OnBannerAdLoad(BANNER_LOADFAILED);
            });
        }

        private void OnBannerAdViewRenderSuccessCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnBannerAdAction != null) ttAdManagerIosClient.OnBannerAdAction(BANNER_RENDERSUCESS);
            });
        }

        private void OnBannerAdViewRenderFailCallback(int code,string msg) {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnBannerAdAction != null) ttAdManagerIosClient.OnBannerAdAction(BANNER_RENDERFAILED);
            });
        }

        private void OnBannerAdViewWillBecomVisibleCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnBannerAdAction != null) ttAdManagerIosClient.OnBannerAdAction(BANNER_SHOW);
            });
        }

        private void OnBannerAdViewDidClickCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnBannerAdAction != null) ttAdManagerIosClient.OnBannerAdAction(BANNER_CLICKED);
            });
        }

        private void OnBannerAdViewDislikeWithReasonCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnBannerAdAction != null) ttAdManagerIosClient.OnBannerAdAction(100);
            });
        }

        private void OnBannerAdViewDidCloseOtherControllerCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnBannerAdAction != null) ttAdManagerIosClient.OnBannerAdAction(101);
            });
        }

        // rewardVideo
        public override void LoadAdRewardVideo(string codeId) {
            Externs.loadAdRewardVideo(codeId);
        }

        public override void ShowAdRewardVideo() {
            Externs.showAdRewardVideo();
        }


        private void OnRewardVideoDidLoadCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnRewardVideoAdLoad != null) ttAdManagerIosClient.OnRewardVideoAdLoad(REWARDVIDEO_LOADED);
            });
        }

        private void OnRewardVideoDidLoadFailCallback( int code,string msg) {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnRewardVideoAdAction != null) ttAdManagerIosClient.OnRewardVideoAdLoad(REWARDVIDEO_LOADFAILED);
            });
        }

        private void OnRewardVideoDidCloseCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnRewardVideoAdLoad != null) ttAdManagerIosClient.OnRewardVideoAdAction(REWARDVIDEO_CLOSE);
            });
        }

        private void OnRewardVideoDidVisibleCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnRewardVideoAdLoad != null) ttAdManagerIosClient.OnRewardVideoAdAction(REWARDVIDEO_SHOW);
            });
        }

        private void OnRewardVideoRenderSuccessCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnRewardVideoAdLoad != null) ttAdManagerIosClient.OnRewardVideoAdAction(100);
            });
        }

        private void OnRewardVideoRenderFailCallback(int code,string msg) {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnRewardVideoAdLoad != null) ttAdManagerIosClient.OnRewardVideoAdAction(REWARDVIDEO_LOADED);
            });
        }

        private void OnRewardVideoDidClickCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnRewardVideoAdLoad != null) ttAdManagerIosClient.OnRewardVideoAdAction(REWARDVIDEO_BARCLICK);
            });
        }

        private void OnRewardVideoDidClickSkipCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnRewardVideoAdLoad != null) ttAdManagerIosClient.OnRewardVideoAdAction(REWARDVIDEO_SKIPPED);
            });
        }

        private void OnRewardVideoServerRewardDidSuccessCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnRewardVideoAdLoad != null) ttAdManagerIosClient.OnRewardVideoAdAction(REWARDVIDEO_REWARD);
            });
        }

        private void OnRewardVideoServerRewardDidFailCallback(int code,string msg) {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnRewardVideoAdLoad != null) ttAdManagerIosClient.OnRewardVideoAdAction(101);
            });
        }


        // fullVideo

        public override void ShowAdFullVideo() {
            Externs.showAdFullVideo();
        }

        public override void LoadAdFullVideo(string codeId) {
            Externs.loadAdFullVideo(codeId);
        }

        private void OnFullVideoDidLoadCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnRewardVideoAdLoad != null) ttAdManagerIosClient.OnFullVideoAdLoad(FULLVIDEO_LOADED);
            });
        }

        private void OnFullVideoDidLoadFailCallback(int code,string msg) {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnRewardVideoAdLoad != null) ttAdManagerIosClient.OnFullVideoAdLoad(FULLVIDEO_LOADFAILED);
            });
        }

        private void OnFullVideoRenderSuccessCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnFullVideoAdAction != null) ttAdManagerIosClient.OnFullVideoAdAction(100);
            });
        }

        private void OnFullVideoRenderFailCallback(int code,string msg) {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnFullVideoAdAction != null) ttAdManagerIosClient.OnFullVideoAdAction(101);
            });
        }

        private void OnFullVideoDidVisibleCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnFullVideoAdAction != null) ttAdManagerIosClient.OnFullVideoAdAction(FULLVIDEO_SHOW);
            });
        }

        private void OnFullVideoDidCloseCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnFullVideoAdAction != null) ttAdManagerIosClient.OnFullVideoAdAction(FULLVIDEO_CLOSE);
            });
        }

        private void OnFullVideoDidClickCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnFullVideoAdAction != null) ttAdManagerIosClient.OnFullVideoAdAction(102);
            });
        }

        private void OnFullVideoDidClickSkipCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnFullVideoAdAction != null) ttAdManagerIosClient.OnFullVideoAdAction(FULLVIDEO_SKIPPED);
            });
        }

        // interstitial

        public override void ShowAdInterstitialVideo() {
            Externs.showAdInterstitialVideo();
        }

        public override void LoadAdInterstitialVideo(string codeId) {
            Externs.loadAdInterstitialVideo(codeId);
        }

        private void OnInterstitialDidLoadCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnInterstitialAdLoad != null) ttAdManagerIosClient.OnInterstitialAdLoad(INTERSTITIAL_LOADED);
            });
        }

        private void OnInterstitialDidLoadFailCallback( int code,string msg) {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnInterstitialAdLoad != null) ttAdManagerIosClient.OnInterstitialAdLoad(INTERSTITIAL_LOADFAILED);
            });
        }

        private void OnInterstitialRenderSuccessCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnInterstitialAdAction != null) ttAdManagerIosClient.OnInterstitialAdAction(INTERSTITIAL_RENDERSUCCESS);
            });
        }

        private void OnInterstitialRenderFailCallback(int code, string msg) {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnInterstitialAdAction != null) ttAdManagerIosClient.OnInterstitialAdAction(INTERSTITIAL_RENDERFAILED);
            });
        }

        private void OnInterstitialWillVisibleCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnInterstitialAdAction != null) ttAdManagerIosClient.OnInterstitialAdAction(INTERSTITIAL_SHOW);
            });
        }

        private void OnInterstitialDidCloseCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnInterstitialAdAction != null) ttAdManagerIosClient.OnInterstitialAdAction(INTERSTITIAL_DISMISSED);
            });
        }

        private void OnInterstitialDidClickCallback() {
            FrameworkEngine.Instance.RunOnMain(() => {
                if (ttAdManagerIosClient.OnInterstitialAdAction != null) ttAdManagerIosClient.OnInterstitialAdAction(INTERSTITIAL_CLICKED);
            });
        }
    }

    public class Externs {

        [DllImport("__Internal")]
        public static extern void init(string appId);

        // Banner

        [DllImport("__Internal")]
        public static extern void loadAdBanner(string codeId);

        [DllImport("__Internal")]
        public static extern void showAdBanner();

        [DllImport("__Internal")]
        internal static extern void setBannerAdViewDidLoadCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setBannerAdViewDidLoadFailCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setBannerAdViewRenderSuccessCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setBannerAdViewRenderFailCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setBannerAdViewWillBecomVisibleCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setBannerAdViewDidClickCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setBannerAdViewDislikeWithReasonCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setBannerAdViewDidCloseOtherControllerCallback(IntPtr callback);

        // RewardVideo

        [DllImport("__Internal")]
        public static extern void loadAdRewardVideo(string codeId);

        [DllImport("__Internal")]
        public static extern void showAdRewardVideo();


        [DllImport("__Internal")]
        internal static extern void setRewardVideoDidLoadCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setRewardVideoDidLoadFailCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setRewardVideoRenderSuccessCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setRewardVideoRenderFailCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setRewardVideoDidCloseCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setRewardVideoDidVisibleCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setRewardVideoDidClickCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setRewardVideoDidClickSkipCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setRewardVideoServerRewardDidSuccessCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setRewardVideoServerRewardDidFailCallback(IntPtr callback);


        // FullVideo

        [DllImport("__Internal")]
        public static extern void showAdFullVideo();

        [DllImport("__Internal")]
        public static extern void loadAdFullVideo(string codeId);

        [DllImport("__Internal")]
        internal static extern void setFullVideoDidLoadCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setFullVideoDidLoadFailCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setFullVideoRenderSuccessCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setFullVideoRenderFailCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setFullVideoDidVisibleCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setFullVideoDidCloseCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setFullVideoDidClickCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setFullVideoDidClickSkipCallback(IntPtr callback);

        // Interstitial

        [DllImport("__Internal")]
        public static extern void showAdInterstitialVideo();

        [DllImport("__Internal")]
        public static extern void loadAdInterstitialVideo(string codeId);

        [DllImport("__Internal")]
        internal static extern void setInterstitialDidLoadCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setInterstitialDidLoadFailCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setInterstitialRenderSuccessCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setInterstitialRenderFailCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setInterstitialWillVisibleCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setInterstitialDidCloseCallback(IntPtr callback);

        [DllImport("__Internal")]
        internal static extern void setInterstitialDidClickCallback(IntPtr callback);
    }
}
