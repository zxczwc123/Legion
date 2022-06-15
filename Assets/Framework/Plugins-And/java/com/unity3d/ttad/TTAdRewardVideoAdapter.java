package com.unity3d.ttad;

import android.app.Activity;
import android.support.annotation.NonNull;
import android.util.Log;
import android.widget.Toast;

import com.bytedance.sdk.openadsdk.AdSlot;
import com.bytedance.sdk.openadsdk.TTAdManager;
import com.bytedance.sdk.openadsdk.TTAdNative;
import com.bytedance.sdk.openadsdk.TTRewardVideoAd;

import java.lang.ref.WeakReference;
import java.util.Map;
import java.util.Set;
import java.util.concurrent.atomic.AtomicBoolean;

public class TTAdRewardVideoAdapter {

    private static final String ADAPTER_NAME = "TTAdRewardVideoAdapter";

    /**
     * Flag to determine whether or not the adapter has been initialized.
     */
    private static AtomicBoolean sIsInitialized;

    /**
     * Pangolin audience network
     */
    private String mCodeId = "901121593";//TTAdConstant.VERTICAL

    private WeakReference<Activity> mWeakActivity;

    private TTRewardVideoAd mTTRewardVideoAd;

    private ITTAdRewardVideoListener adRewardVideoListener;

    /**
     * Flag to determine whether or not the Google Rewarded Video Ad instance has loaded.
     */
    private boolean mIsLoaded;

    public TTAdRewardVideoAdapter() {
        sIsInitialized = new AtomicBoolean(false);
        Log.i(ADAPTER_NAME, "TTAdRewardVideoAdapter has been create ....");
    }

    public void setListener(ITTAdRewardVideoListener listener){
        this.adRewardVideoListener = listener;
    }

    public boolean hasVideoAvailable() {
        return mTTRewardVideoAd != null && mIsLoaded;
    }

    /**
     * 加载激励视频广告
     *
     * @throws Exception
     */
    public void loadRewardedAd(@NonNull Activity activity,String codeId)  {
        Log.i( ADAPTER_NAME, "loadWithSdkInitialized method execute ......");
        mWeakActivity = new WeakReference<>(activity);

        mCodeId = codeId;

        TTAdManagerHolder.init(activity);

        TTAdManager mTTAdManager = TTAdManagerHolder.get();
        TTAdNative mTTAdNative = mTTAdManager.createAdNative(activity.getApplicationContext());
        AdSlot adSlot = new AdSlot.Builder()
                .setCodeId(mCodeId)
                .setImageAcceptedSize(1080, 1920)
                .build();

        //load ad
        mTTAdNative.loadRewardVideoAd(adSlot, mLoadRewardVideoAdListener);
    }

    public void showVideo() {
        Log.i( ADAPTER_NAME,"showVideo");
        if (hasVideoAvailable() && mWeakActivity != null && mWeakActivity.get() != null) {
            mTTRewardVideoAd.setRewardAdInteractionListener(mRewardAdInteractionListener);
            mTTRewardVideoAd.showRewardVideoAd(mWeakActivity.get());
        } else {
            Log.i( ADAPTER_NAME, "showVideo fail");
        }
    }

    public String getAdNetworkId() {
        return mCodeId;
    }

    public void invalidate() {
        if (mTTRewardVideoAd != null) {
            Log.i( ADAPTER_NAME, "Performing cleanup tasks...");
            mTTRewardVideoAd = null;
        }
    }

    private TTAdNative.RewardVideoAdListener mLoadRewardVideoAdListener = new TTAdNative.RewardVideoAdListener() {

        @Override
        public void onError(int code, String message) {
            Log.i( ADAPTER_NAME, "Loading Rewarded Video creative encountered an error: " + code + ",error message:" + message);
            if(adRewardVideoListener != null){
                adRewardVideoListener.onRewardVideoLoadFailed(code,message);
            }
        }

        @Override
        public void onRewardVideoAdLoad(TTRewardVideoAd ad) {
            Log.i( ADAPTER_NAME, " TTRewardVideoAd ：" + ad);
            if (ad != null) {
                mIsLoaded = true;
                mTTRewardVideoAd = ad;
                Log.i( ADAPTER_NAME,"TTRewardVideoAd loaded");
                if(adRewardVideoListener != null){
                    adRewardVideoListener.onRewardVideoLoaded();
                }
            } else {
                Log.i( ADAPTER_NAME, " TTRewardVideoAd is null !");
                if(adRewardVideoListener != null){
                    adRewardVideoListener.onRewardVideoLoadFailed(0,null);
                }
            }
        }

        @Override
        public void onRewardVideoCached() {
            Log.i( ADAPTER_NAME, "TTRewardVideoAd onRewardVideoCached...");
        }
    };

    private TTRewardVideoAd.RewardAdInteractionListener mRewardAdInteractionListener = new TTRewardVideoAd.RewardAdInteractionListener() {
        @Override
        public void onAdShow() {
            Log.i( ADAPTER_NAME, "TTRewardVideoAd onAdShow...");
            if(adRewardVideoListener != null){
                adRewardVideoListener.onRewardVideoShow();
            }
        }

        @Override
        public void onAdVideoBarClick() {
            Log.i( ADAPTER_NAME, "TTRewardVideoAd onAdVideoBarClick...");
            if(adRewardVideoListener != null){
                adRewardVideoListener.onRewardVideoBarClick();
            }
        }

        @Override
        public void onAdClose() {
            Log.i( ADAPTER_NAME, "TTRewardVideoAd onAdClose...");
            if(adRewardVideoListener != null){
                adRewardVideoListener.onRewardVideoClose();
            }
        }

        @Override
        public void onVideoComplete() {
            Log.i( ADAPTER_NAME, "TTRewardVideoAd onVideoComplete...");
            if(adRewardVideoListener != null){
                adRewardVideoListener.onRewardVideoComplete();
            }
        }

        @Override
        public void onVideoError() {
            Log.i( ADAPTER_NAME, "TTRewardVideoAd onVideoError...");
            if(adRewardVideoListener != null){
                adRewardVideoListener.onRewardVideoError();
            }
        }

        @Override
        public void onRewardVerify(boolean rewardVerify, int rewardAmount, String rewardName, int errorCode, String errorMsg) {
            Log.i( ADAPTER_NAME, "TTRewardVideoAd onRewardVerify...rewardVerify：" + rewardVerify + "，rewardAmount=" + rewardAmount + "，rewardName=" + rewardName);
            if(adRewardVideoListener != null){
                adRewardVideoListener.onRewardVideoRewardVertify(rewardVerify,rewardAmount,rewardName,errorCode,errorMsg);
            }
        }

        @Override
        public void onSkippedVideo() {
            Log.i( ADAPTER_NAME, "TTRewardVideoAd onSkippedVideo...");
            if(adRewardVideoListener != null){
                adRewardVideoListener.onRewardVideoSkipped();
            }
        }
    };
}
