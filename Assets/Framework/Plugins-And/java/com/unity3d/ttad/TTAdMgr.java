package com.unity3d.ttad;

import com.unity3d.player.UnityPlayer;

public class TTAdMgr {

    private static TTAdBannerAdapter ttAdBannerAdapter;

    private static TTAdFullVideoAdapter ttAdFullVideoAdapter;

    private static TTAdRewardVideoAdapter ttAdRewardVideoAdapter;

    private static TTAdInterstitialAdapter ttAdInterstitialAdapter;

    public static void init(){
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                TTAdManagerHolder.init(UnityPlayer.currentActivity);
            }
        });
    }

    public static void init(final String appId){
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                TTAdManagerHolder.init(UnityPlayer.currentActivity,appId);
            }
        });
    }

    public static void setTTAdBannerListener(ITTAdBannerListener listener){
        if(ttAdBannerAdapter == null){
            ttAdBannerAdapter = new TTAdBannerAdapter();
        }
        ttAdBannerAdapter.setListener(listener);
    }

    public static void setTTAdFullVideoListener(ITTAdFullVideoListener listener){
        if(ttAdFullVideoAdapter == null){
            ttAdFullVideoAdapter = new TTAdFullVideoAdapter();
        }
        ttAdFullVideoAdapter.setListener(listener);
    }

    public static void setTTAdRewardVideoListener(ITTAdRewardVideoListener listener){
        if(ttAdRewardVideoAdapter == null){
            ttAdRewardVideoAdapter = new TTAdRewardVideoAdapter();
        }
        ttAdRewardVideoAdapter.setListener(listener);
    }

    public static void setTTAdInterstitialListener(ITTAdInterstitialListener listener){
        if(ttAdInterstitialAdapter == null){
            ttAdInterstitialAdapter = new TTAdInterstitialAdapter();
        }
        ttAdInterstitialAdapter.setListener(listener);
    }


    public static void loadBannerAd(final String codeId){
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if(ttAdBannerAdapter == null){
                    ttAdBannerAdapter = new TTAdBannerAdapter();
                }
                ttAdBannerAdapter.loadBanner(UnityPlayer.currentActivity,codeId);
            }
        });
    }



    public static void loadRewardVideoAd(final String codeId){
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if(ttAdRewardVideoAdapter == null){
                    ttAdRewardVideoAdapter = new TTAdRewardVideoAdapter();
                }
                ttAdRewardVideoAdapter.loadRewardedAd(UnityPlayer.currentActivity,codeId);
            }
        });
    }

    public static void showRewardVideoAd(){
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if(ttAdRewardVideoAdapter == null){
                    ttAdRewardVideoAdapter = new TTAdRewardVideoAdapter();
                }
                ttAdRewardVideoAdapter.showVideo();
            }
        });
    }

    public static void loadInterstitialAd(final String codeId){
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if(ttAdInterstitialAdapter == null){
                    ttAdInterstitialAdapter = new TTAdInterstitialAdapter();
                }
                ttAdInterstitialAdapter.loadInterstitial(UnityPlayer.currentActivity,codeId);
            }
        });
    }

    public static void showInterstitialAd(){
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if(ttAdInterstitialAdapter == null){
                    ttAdInterstitialAdapter = new TTAdInterstitialAdapter();
                }
                ttAdInterstitialAdapter.showInterstitial();
            }
        });
    }

    public static void loadFullVideoAd(final String codeId){
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if(ttAdFullVideoAdapter == null){
                    ttAdFullVideoAdapter = new TTAdFullVideoAdapter();
                }
                ttAdFullVideoAdapter.loadFullVideo(UnityPlayer.currentActivity,codeId);
            }
        });
    }

    public static void showFullVideoAd(){
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if(ttAdFullVideoAdapter == null){
                    ttAdFullVideoAdapter = new TTAdFullVideoAdapter();
                }
                ttAdFullVideoAdapter.showFullVideo();
            }
        });
    }
}
