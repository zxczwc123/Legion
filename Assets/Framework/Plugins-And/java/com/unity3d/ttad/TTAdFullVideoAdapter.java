package com.unity3d.ttad;

import android.app.Activity;
import android.support.annotation.NonNull;
import android.util.Log;

import com.bytedance.sdk.openadsdk.AdSlot;
import com.bytedance.sdk.openadsdk.TTAdManager;
import com.bytedance.sdk.openadsdk.TTAdNative;
import com.bytedance.sdk.openadsdk.TTFullScreenVideoAd;

import java.lang.ref.WeakReference;
import java.util.concurrent.atomic.AtomicBoolean;

public class TTAdFullVideoAdapter {

    private static final String ADAPTER_NAME = "TTAdFullVideoAdapter";

    /**
     * Flag to determine whether or not the adapter has been initialized.
     */
    private static AtomicBoolean sIsInitialized;

    private ITTAdFullVideoListener adFullVideoListener;

    /**
     * pangolin audience network Mobile Ads rewarded video ad unit ID.
     */
    private String mCodeId = "901121073";//TTAdConstant.VERTICAL

    private WeakReference<Activity> mWeakActivity;

    private TTFullScreenVideoAd mTTFullScreenVideoAd;

    /**
     * Flag to determine whether or not the Pangolin Rewarded Video Ad instance has loaded.
     */
    private boolean mIsLoaded;

    public TTAdFullVideoAdapter() {
        sIsInitialized = new AtomicBoolean(false);
        Log.i(ADAPTER_NAME,"PangolinAudienceAdFullVideoAdapter has been create ....");
    }

    public void setListener(ITTAdFullVideoListener listener){

    }

    public String getAdNetworkId() {
        return mCodeId;
    }

    public boolean hasVideoAvailable() {
        return mTTFullScreenVideoAd != null && mIsLoaded;
    }

    public void loadFullVideo(@NonNull Activity activity,String codeId)  {
        Log.i( ADAPTER_NAME, "loadWithSdkInitialized method execute ......");
        this.mCodeId = codeId;
        mWeakActivity = new WeakReference<>(activity);
        TTAdManagerHolder.init(activity.getApplicationContext());
        TTAdManager mTTAdManager = TTAdManagerHolder.get();
        TTAdNative mTTAdNative = mTTAdManager.createAdNative(activity.getApplicationContext());
        //Create a parameter AdSlot for reward ad request type,
        //refer to the document for meanings of specific parameters
        AdSlot adSlot = new AdSlot.Builder()
                .setCodeId(mCodeId)
                .setImageAcceptedSize(1080, 1920)
                .build();
        //load ad
        mTTAdNative.loadFullScreenVideoAd(adSlot, mLoadFullVideoAdListener);
    }

    public void showFullVideo() {
        Log.i( ADAPTER_NAME,"showVideo");
        if (hasVideoAvailable() && mWeakActivity != null && mWeakActivity.get() != null) {
            mTTFullScreenVideoAd.setFullScreenVideoAdInteractionListener(mFullScreenVideoAdInteractionListener);
            mTTFullScreenVideoAd.showFullScreenVideoAd(mWeakActivity.get());
        } else {
            Log.i( ADAPTER_NAME, "show error");
        }
    }

    public void invalidate() {
        if (mTTFullScreenVideoAd != null) {
            Log.i( ADAPTER_NAME, "Performing cleanup tasks...");
            mTTFullScreenVideoAd = null;
        }
    }

    private TTAdNative.FullScreenVideoAdListener mLoadFullVideoAdListener = new TTAdNative.FullScreenVideoAdListener() {
        @Override
        public void onError(int code, String message) {
            Log.i(ADAPTER_NAME,"Loading Full Video creative encountered an error: " + code + ",error message:" + message);
            if(adFullVideoListener != null){
                adFullVideoListener.onFullVideoLoadFailed(code,message);
            }
        }

        @Override
        public void onFullScreenVideoAdLoad(TTFullScreenVideoAd ad) {
            Log.i(ADAPTER_NAME,"onFullScreenVideoAdLoad method execute ......ad = " + ad);
            if (ad != null) {
                mIsLoaded = true;
                mTTFullScreenVideoAd = ad;
                Log.i( ADAPTER_NAME,"onRewardedVideoLoadSuccess");
                if(adFullVideoListener != null){
                    adFullVideoListener.onFullVideoLoaded();
                }
            } else {
                Log.i( ADAPTER_NAME, " mTTFullScreenVideoAd is null !");
                if(adFullVideoListener != null){
                    adFullVideoListener.onFullVideoLoadFailed(0,null);
                }
            }
        }

        @Override
        public void onFullScreenVideoCached() {
            Log.i( ADAPTER_NAME, " mTTFullScreenVideoAd onFullScreenVideoCached invoke !");
        }
    };

    private TTFullScreenVideoAd.FullScreenVideoAdInteractionListener mFullScreenVideoAdInteractionListener = new TTFullScreenVideoAd.FullScreenVideoAdInteractionListener() {

        @Override
        public void onAdShow() {
            Log.i( ADAPTER_NAME, "TTFullScreenVideoAd onAdShow...");

            if(adFullVideoListener != null){
                adFullVideoListener.onFullVideoShow();
            }
        }

        @Override
        public void onAdVideoBarClick() {
            Log.i( ADAPTER_NAME, "TTFullScreenVideoAd onAdVideoBarClick...");

            if(adFullVideoListener != null){
                adFullVideoListener.onFullVideoAdVideoBarClick();
            }
        }

        @Override
        public void onAdClose() {
            Log.i( ADAPTER_NAME, "TTFullScreenVideoAd onAdClose...");
            if(adFullVideoListener != null){
                adFullVideoListener.onFullVideoAdClose();
            }
        }

        @Override
        public void onVideoComplete() {
            Log.i( ADAPTER_NAME, "TTFullScreenVideoAd onVideoComplete...");
            if(adFullVideoListener != null){
                adFullVideoListener.onFullVideoComplete();
            }
        }

        @Override
        public void onSkippedVideo() {
            Log.i( ADAPTER_NAME, "TTFullScreenVideoAd onSkippedVideo...");

            if(adFullVideoListener != null){
                adFullVideoListener.onFullVideoSkipped();
            }
        }
    };
}
