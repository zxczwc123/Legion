package com.unity3d.ttad;

import android.app.Activity;
import android.content.Context;
import android.util.Log;
import android.view.View;

import com.bytedance.sdk.openadsdk.AdSlot;
import com.bytedance.sdk.openadsdk.TTAdManager;
import com.bytedance.sdk.openadsdk.TTAdNative;
import com.bytedance.sdk.openadsdk.TTNativeExpressAd;

import java.util.List;
import java.util.concurrent.atomic.AtomicBoolean;

public class TTAdInterstitialAdapter {

    private static final String ADAPTER_NAME = "TTAdInterstitialAdapter";

    //for pangolin ad network key
    public final static String EXPRESS_VIEW_WIDTH = "express_view_width";
    public final static String EXPRESS_VIEW_HEIGHT = "express_view_height";
    public final static String EXPRESS_ACTIVITY_PARAM = "activity_param";
    private String mCodeId = "901121133";

    private ITTAdInterstitialListener mInterstitialListener;

    private TTNativeExpressAd mTTInterstitialExpressAd;

    private Context mContext;
    private Activity mActivity;

    private AtomicBoolean isRenderLoaded = new AtomicBoolean(false);

    public TTAdInterstitialAdapter() {
        Log.i(ADAPTER_NAME, "PangolinAudienceAdAdapterInterstitial has been create ....");
    }

    public void setListener(ITTAdInterstitialListener listener){
        this.mInterstitialListener = listener;
    }

    public void loadInterstitial(Context context,String codeId) {
        this.mContext = context;
        this.mCodeId = codeId;
        //创建TTAdManager
        TTAdManager ttAdManager = TTAdManagerHolder.get();
        TTAdNative ttAdNative = ttAdManager.createAdNative(context.getApplicationContext());

        //获取宽高参数
        float expressViewWidth = 0;
        float expressViewHeight = 0;

        if (expressViewWidth <= 0) {
            expressViewWidth = 350;
            expressViewHeight = 0;//0自适应
        }

        if (expressViewHeight < 0) {
            expressViewHeight = 0;
        }

        //step4:创建广告请求参数AdSlot,具体参数含义参考文档
        AdSlot adSlot = new AdSlot.Builder()
                .setCodeId(mCodeId) //广告位id
                .setAdCount(1) //请求广告数量为1到3条
                .setExpressViewAcceptedSize(expressViewWidth, expressViewHeight) //期望模板广告view的size,单位dp
                .build();
        //step5:请求广告，对请求回调的广告作渲染处理
        ttAdNative.loadInteractionExpressAd(adSlot, mInterstitialAdExpressAdListener);

    }

    public void showInterstitial() {
        Log.i( ADAPTER_NAME,"showInterstitial");
        if (mTTInterstitialExpressAd != null && isRenderLoaded.get()) {
            mTTInterstitialExpressAd.showInteractionExpressAd(mActivity);
        } else {
            Log.i( ADAPTER_NAME,"NETWORK_NO_FILL");

            if (mInterstitialListener != null) {
                mInterstitialListener.onInterstitialRenderFailed(null,0);
            }
        }
    }

    public void invalidate() {
        if (mTTInterstitialExpressAd != null) {
            mTTInterstitialExpressAd.setExpressInteractionListener(null);
            mTTInterstitialExpressAd.setVideoAdListener(null);
            mTTInterstitialExpressAd.destroy();
            mTTInterstitialExpressAd = null;
        }
    }

    /**
     * pangolin ad 动态布局插屏请求监听器
     */
    private TTAdNative.NativeExpressAdListener mInterstitialAdExpressAdListener = new TTAdNative.NativeExpressAdListener() {
        @Override
        public void onError(int code, String message) {
            Log.i( ADAPTER_NAME,  message);
            if (mInterstitialListener != null) {
                mInterstitialListener.onInterstitialLoadFailed(code,message);
            }
        }

        @Override
        public void onNativeExpressAdLoad(List<TTNativeExpressAd> ads) {
            if (ads == null || ads.size() == 0) {
                return;
            }
            mTTInterstitialExpressAd = ads.get(0);
            mTTInterstitialExpressAd.setSlideIntervalTime(30 * 1000);
            mTTInterstitialExpressAd.setExpressInteractionListener(mInterstitialExpressAdInteractionListener);
            mTTInterstitialExpressAd.render();
            if (mInterstitialListener != null) {
                mInterstitialListener.onInterstitialLoaded();
            }
        }
    };

    /**
     * 渲染回调监听器
     */
    private TTNativeExpressAd.AdInteractionListener mInterstitialExpressAdInteractionListener = new TTNativeExpressAd.AdInteractionListener() {
        @Override
        public void onAdDismiss() {
            if (mInterstitialListener != null){
                mInterstitialListener.onInterstitialDismissed();
            }
        }

        @Override
        public void onAdClicked(View view, int type) {
            if (mInterstitialListener != null){
                mInterstitialListener.onInterstitialClicked();
            }
        }

        @Override
        public void onAdShow(View view, int type) {
            if (mInterstitialListener != null){
                mInterstitialListener.onInterstitialShow();
            }
        }

        @Override
        public void onRenderFail(View view, String msg, int code) {
            Log.i(ADAPTER_NAME, msg);
            if (mInterstitialListener != null) {
                mInterstitialListener.onInterstitialRenderFailed(msg,code);
            }
        }

        @Override
        public void onRenderSuccess(View view, float width, float height) {
            //返回view的宽高 单位 dp
            isRenderLoaded.set(true);
            if (mInterstitialListener != null) {
                mInterstitialListener.onInterstitialRenderSuccess();
            }
        }
    };



}
