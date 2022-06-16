using Framework.Core;
using System;
using UnityEngine;

namespace Framework.Native.TTAd {
    public class TTAdManagerClientAnd : TTAdManager {

        private AndroidJavaClass ttAdMgrClass;

        private bool isBannerInit;

        private bool isFullVideoInit;

        private bool isRewardVideoInit;

        private bool isInterstitialInit;

        public TTAdManagerClientAnd() {
            this.ttAdMgrClass = new AndroidJavaClass("com.unity3d.ttad.TTAdMgr");
        }

        public override void Init(string appId) {
            ttAdMgrClass.CallStatic("init", appId);
        }

        public override void Unload() {
            this.ttAdMgrClass.CallStatic("setTTAdBannerListener", null);
            this.ttAdMgrClass.CallStatic("setTTAdFullVideoListener", null);
            this.ttAdMgrClass.CallStatic("setTTAdRewardVideoListener", null);
            this.ttAdMgrClass.CallStatic("setTTAdInterstitialListener", null);
        }

        private void InitBanner() {
            if (!this.isBannerInit) {
                var adListener = new AdBannerListener(this);
                this.ttAdMgrClass.CallStatic("setTTAdBannerListener", adListener);
                this.isBannerInit = true;
            }
        }

        private void InitFullVideo() {
            if (!this.isFullVideoInit) {
                var adListener = new AdFullVideoListener(this);
                this.ttAdMgrClass.CallStatic("setTTAdFullVideoListener", adListener);
                this.isFullVideoInit = true;
            }
        }

        private void InitRewardVideo() {
            if (!this.isRewardVideoInit) {
                var adListener = new AdRewardVideoListener(this);
                this.ttAdMgrClass.CallStatic("setTTAdRewardVideoListener", adListener);
                this.isRewardVideoInit = true;
            }
        }

        private void InitInterstitial() {
            if (!this.isInterstitialInit) {
                var adListener = new AdInterstitialListener(this);
                this.ttAdMgrClass.CallStatic("setTTAdInterstitialListener", adListener);
                this.isInterstitialInit = true;
            }
        }

        public override void LoadAdBanner(string codeId) {
            this.InitBanner();
            ttAdMgrClass.CallStatic("loadBannerAd", codeId);
        }

        public override void ShowAdBanner() {
            
        }


        public override void LoadAdRewardVideo(string codeId) {
            this.InitRewardVideo();
            ttAdMgrClass.CallStatic("loadRewardVideoAd", codeId);
        }

        public override void ShowAdRewardVideo() {
            ttAdMgrClass.CallStatic("showRewardVideoAd");
        }

        public override void LoadAdFullVideo(string codeId) {
            this.InitFullVideo();
            ttAdMgrClass.CallStatic("loadFullVideoAd", codeId);
        }

        public override void ShowAdFullVideo() {
            ttAdMgrClass.CallStatic("showFullVideoAd");
        }

        public override void LoadAdInterstitialVideo(string codeId) {
            this.InitInterstitial();
            ttAdMgrClass.CallStatic("loadInterstitialAd", codeId);
        }

        public override void ShowAdInterstitialVideo() {
            ttAdMgrClass.CallStatic("showInterstitialAd");
        }

        public void onBannerFailed(int code, String msg) {
            Engine.instance.RunOnMain(() => {
                if (this.OnBannerAdLoad != null) this.OnBannerAdLoad(TTAdManager.BANNER_LOADFAILED);
            });
        }

        public void onBannerLoaded() {
            Engine.instance.RunOnMain(() => {
                if (this.OnBannerAdLoad != null) this.OnBannerAdLoad(TTAdManager.BANNER_LOADED);
            });
        }

        public void onBannerClicked() {
            Engine.instance.RunOnMain(() => {
                if (this.OnBannerAdAction != null) this.OnBannerAdAction(TTAdManager.BANNER_LOADED);
            });
        }

        public void onBannerShow() {
            Engine.instance.RunOnMain(() => {
                if (this.OnBannerAdAction != null) this.OnBannerAdAction(TTAdManager.BANNER_LOADED);
            });
        }

        public void onBannerRenderFailed(String msg, int code) {
            Engine.instance.RunOnMain(() => {
                if (this.OnBannerAdAction != null) this.OnBannerAdAction(TTAdManager.BANNER_LOADED);
            });
        }

        public void onBannerRenderSuccess() {
            Engine.instance.RunOnMain(() => {
                if (this.OnBannerAdAction != null) this.OnBannerAdAction(TTAdManager.BANNER_LOADED);
            });
        }

        public void onRewardVideoLoadFailed(int code, String message) {
            Engine.instance.RunOnMain(() => {
                if (this.OnRewardVideoAdLoad != null) this.OnRewardVideoAdLoad(TTAdManager.REWARDVIDEO_LOADFAILED);
            });
        }

        public void onRewardVideoLoaded() {
            Engine.instance.RunOnMain(() => {
                if (this.OnRewardVideoAdLoad != null) this.OnRewardVideoAdLoad(TTAdManager.REWARDVIDEO_LOADED);
            });
        }

        public void onRewardVideoShow() {
            Engine.instance.RunOnMain(() => {
                if (this.OnRewardVideoAdAction != null) this.OnRewardVideoAdAction(TTAdManager.REWARDVIDEO_SHOW);
            });
        }

        public void onRewardVideoBarClick() {
            Engine.instance.RunOnMain(() => {
                if (this.OnRewardVideoAdAction != null) this.OnRewardVideoAdAction(TTAdManager.REWARDVIDEO_BARCLICK);
            });
        }

        public void onRewardVideoClose() {
            Engine.instance.RunOnMain(() => {
                if (this.OnRewardVideoAdAction != null) this.OnRewardVideoAdAction(TTAdManager.REWARDVIDEO_CLOSE);
            });
        }

        public void onRewardVideoComplete() {
            Engine.instance.RunOnMain(() => {
                if (this.OnRewardVideoAdAction != null) this.OnRewardVideoAdAction(TTAdManager.REWARDVIDEO_COMPLETE);
            });
        }

        public void onRewardVideoError() {
            Engine.instance.RunOnMain(() => {
                if (this.OnRewardVideoAdAction != null) this.OnRewardVideoAdAction(TTAdManager.REWARDVIDEO_ERROR);
            });
        }

        public void onRewardVideoRewardVertify(bool rewardVerify, int rewardAmount, String rewardName, int errorCode, String errorMsg) {
            Engine.instance.RunOnMain(() => {
                if (this.OnRewardVideoAdAction != null) this.OnRewardVideoAdAction(TTAdManager.REWARDVIDEO_REWARD);
            });
        }

        public void onRewardVideoSkipped() {
            Engine.instance.RunOnMain(() => {
                if (this.OnRewardVideoAdAction != null) this.OnRewardVideoAdAction(TTAdManager.REWARDVIDEO_SKIPPED);
            });
        }

        
    }

    public class AdBannerListener : AndroidJavaProxy {

        private TTAdManagerClientAnd managerClient;

        public AdBannerListener(TTAdManagerClientAnd managerClient) : base("com.unity3d.ttad.ITTAdBannerListener") {
            this.managerClient = managerClient;
        }

        public void onBannerFailed(int code, String msg) {
            this.managerClient.onBannerFailed(code, msg);
        }

        public void onBannerLoaded() {
            this.managerClient.onBannerLoaded();
        }

        public void onBannerClicked() {
            this.managerClient.onBannerClicked();
        }

        public void onBannerShow() {
            this.managerClient.onBannerShow();
        }

        public void onBannerRenderFailed(String msg, int code) {
            this.managerClient.onBannerRenderFailed(msg, code);
        }

        public void onBannerRenderSuccess() {
            this.managerClient.onBannerRenderSuccess();
        }
    }

    public class AdRewardVideoListener : AndroidJavaProxy {

        private TTAdManagerClientAnd managerClient;

        public AdRewardVideoListener(TTAdManagerClientAnd managerClient) : base("com.unity3d.ttad.ITTAdRewardVideoListener") {
            this.managerClient = managerClient;
        }

        public void onRewardVideoLoadFailed(int code, String message) {
            this.managerClient.onRewardVideoLoadFailed(code,message);
        }

        public void onRewardVideoLoaded() {
            this.managerClient.onRewardVideoLoaded();
        }

        public void onRewardVideoShow() {
            this.managerClient.onRewardVideoShow();
        }

        public void onRewardVideoBarClick() {
            this.managerClient.onRewardVideoBarClick();
        }

        public void onRewardVideoClose() {
            this.managerClient.onRewardVideoClose();
        }

        public void onRewardVideoComplete() {
            this.managerClient.onRewardVideoComplete();
        }

        public void onRewardVideoError() {
            this.managerClient.onRewardVideoError();
        }

        public void onRewardVideoRewardVertify(bool rewardVerify, int rewardAmount, String rewardName, int errorCode, String errorMsg) {
            this.managerClient.onRewardVideoRewardVertify(rewardVerify,rewardAmount,rewardName,errorCode,errorMsg);
        }

        public void onRewardVideoSkipped() {
            this.managerClient.onRewardVideoSkipped();
        }
    }

    public class AdFullVideoListener : AndroidJavaProxy {

        private TTAdManagerClientAnd managerClient;

        public AdFullVideoListener(TTAdManagerClientAnd managerClient) : base("com.unity3d.ttad.ITTAdFullVideoListener") {
            this.managerClient = managerClient;
        }

        public void onFullVideoLoadFailed(int code, String message) {
            Engine.instance.RunOnMain(() => {
                if (this.managerClient.OnFullVideoAdLoad != null) this.managerClient.OnFullVideoAdLoad(TTAdManager.FULLVIDEO_LOADFAILED);
            });
        }

        public void onFullVideoLoaded() {
            Engine.instance.RunOnMain(() => {
                if (this.managerClient.OnFullVideoAdLoad != null) this.managerClient.OnFullVideoAdLoad(TTAdManager.FULLVIDEO_LOADED);
            });
        }

        public void onFullVideoShow() {
            Engine.instance.RunOnMain(() => {
                if (this.managerClient.OnFullVideoAdAction != null) this.managerClient.OnFullVideoAdAction(TTAdManager.FULLVIDEO_SHOW);
            });
        }

        public void onFullVideoAdVideoBarClick() {
            Engine.instance.RunOnMain(() => {
                if (this.managerClient.OnFullVideoAdAction != null) this.managerClient.OnFullVideoAdAction(TTAdManager.FULLVIDEO_BARCLICK);
            });
        }

        public void onFullVideoAdClose() {
            Engine.instance.RunOnMain(() => {
                if (this.managerClient.OnFullVideoAdAction != null) this.managerClient.OnFullVideoAdAction(TTAdManager.FULLVIDEO_CLOSE);
            });
        }

        public void onFullVideoComplete() {
            Engine.instance.RunOnMain(() => {
                if (this.managerClient.OnFullVideoAdAction != null) this.managerClient.OnFullVideoAdAction(TTAdManager.FULLVIDEO_COMPLETE);
            });
        }

        public void onFullVideoSkipped() {
            Engine.instance.RunOnMain(() => {
                if (this.managerClient.OnFullVideoAdAction != null) this.managerClient.OnFullVideoAdAction(TTAdManager.FULLVIDEO_SKIPPED);
            });
        }
    }

    public class AdInterstitialListener : AndroidJavaProxy {

        private TTAdManagerClientAnd managerClient;

        public AdInterstitialListener(TTAdManagerClientAnd managerClient) : base("com.unity3d.ttad.ITTAdInterstitialListener") {
            this.managerClient = managerClient;
        }

        public void onInterstitialLoadFailed(int code, String message) {
            Engine.instance.RunOnMain(() => {
                if (this.managerClient.OnInterstitialAdLoad != null) this.managerClient.OnInterstitialAdLoad(TTAdManager.INTERSTITIAL_LOADFAILED);
            });
        }

        public void onInterstitialLoaded() {
            Engine.instance.RunOnMain(() => {
                if (this.managerClient.OnInterstitialAdLoad != null) this.managerClient.OnInterstitialAdLoad(TTAdManager.INTERSTITIAL_LOADED);
            });
        }

        public void onInterstitialDismissed() {
            Engine.instance.RunOnMain(() => {
                if (this.managerClient.OnInterstitialAdAction != null) this.managerClient.OnInterstitialAdAction(TTAdManager.INTERSTITIAL_DISMISSED);
            });
        }

        public void onInterstitialClicked() {
            Engine.instance.RunOnMain(() => {
                if (this.managerClient.OnInterstitialAdAction != null) this.managerClient.OnInterstitialAdAction(TTAdManager.INTERSTITIAL_CLICKED);
            });
        }

        public void onInterstitialShow() {
            Engine.instance.RunOnMain(() => {
                if (this.managerClient.OnInterstitialAdAction != null) this.managerClient.OnInterstitialAdAction(TTAdManager.INTERSTITIAL_SHOW);
            });
        }

        public void onInterstitialRenderFailed(String message, int code) {
            Engine.instance.RunOnMain(() => {
                if (this.managerClient.OnInterstitialAdAction != null) this.managerClient.OnInterstitialAdAction(TTAdManager.INTERSTITIAL_RENDERFAILED);
            });
        }

        public void onInterstitialRenderSuccess() {
            Engine.instance.RunOnMain(() => {
                if (this.managerClient.OnInterstitialAdAction != null) this.managerClient.OnInterstitialAdAction(TTAdManager.INTERSTITIAL_RENDERSUCCESS);
            });
        }
    }
}
