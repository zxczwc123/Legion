package com.unity3d.ttad;

import android.content.Context;
import android.util.Log;
import android.view.View;

import com.bytedance.sdk.openadsdk.AdSlot;
import com.bytedance.sdk.openadsdk.TTAdManager;
import com.bytedance.sdk.openadsdk.TTAdNative;
import com.bytedance.sdk.openadsdk.TTNativeExpressAd;

import java.util.List;

public class TTAdBannerAdapter {
    private static final String ADAPTER_NAME = "TTAdBannerAdapter";


    /**
     * pangolin audience network Mobile Ads rewarded video ad unit ID.
     */
    private String mCodeId = "901121246";

    public final static String EXPRESS_VIEW_WIDTH = "express_view_width";
    public final static String EXPRESS_VIEW_HEIGHT = "express_view_height";

    private TTNativeExpressAd mTTNativeExpressAd;

    private ITTAdBannerListener customEventBannerListener;

    private Context mContent;

    public TTAdBannerAdapter() {
        Log.i( ADAPTER_NAME, "PangolinAudienceAdBannerAdapter has been create ....");
    }

    public void setListener(ITTAdBannerListener listener){
        this.customEventBannerListener = listener;
    }

    public void loadBanner(Context context,String codeId) {
        Log.i( ADAPTER_NAME, "loadBanner method execute ......");

        this.mCodeId = codeId;
//        this.customEventBannerListener = customEventBannerListener;
        TTAdManagerHolder.init(context);
        TTAdManager mTTAdManager = TTAdManagerHolder.get();
        TTAdNative mTTAdNative = mTTAdManager.createAdNative(context.getApplicationContext());
        //获取宽高参数
        float expressViewWidth = 0;
        float expressViewHeight = 350;


        if (expressViewWidth <= 0) {
            expressViewWidth = 350;
            expressViewHeight = 0;//0自适应
        }

        if (expressViewHeight < 0) {
            expressViewHeight = 0;
        }
        Log.i( ADAPTER_NAME, "expressViewHeight ="+expressViewHeight +"，expressViewWidth="+expressViewWidth);

        //step4:创建广告请求参数AdSlot,具体参数含义参考文档
        AdSlot adSlot = new AdSlot.Builder()
                .setCodeId(mCodeId) //广告位id
                .setAdCount(1) //请求广告数量为1到3条
                .setExpressViewAcceptedSize(expressViewWidth, expressViewHeight) //期望模板广告view的size,单位dp
                .setImageAcceptedSize((int) expressViewWidth, (int) expressViewHeight)
                .build();
        //step5:请求广告，对请求回调的广告作渲染处理
        mTTAdNative.loadBannerExpressAd(adSlot, mTTNativeExpressAdListener);

    }

    public void invalidate() {
        if (mTTNativeExpressAd != null) {
            mTTNativeExpressAd.destroy();
            mTTNativeExpressAd = null;
        }
    }

    /**
     * banner 广告加载回调监听
     */
    private TTAdNative.NativeExpressAdListener mTTNativeExpressAdListener = new TTAdNative.NativeExpressAdListener() {
        @Override
        public void onError(int code, String message) {
            Log.e(ADAPTER_NAME, "MoPubView onBannerFailed.-code=" + code + "," + message);
            if (customEventBannerListener != null) {
                customEventBannerListener.onBannerFailed(code,message);
            }
        }

        @Override
        public void onNativeExpressAdLoad(List<TTNativeExpressAd> ads) {
            if (ads == null || ads.size() == 0) {
                customEventBannerListener.onBannerFailed(0,null);
                return;
            }
            mTTNativeExpressAd = ads.get(0);
            mTTNativeExpressAd.setSlideIntervalTime(30 * 1000);
            mTTNativeExpressAd.setExpressInteractionListener(mExpressAdInteractionListener);
            mTTNativeExpressAd.render();
            if (customEventBannerListener != null) {
                customEventBannerListener.onBannerLoaded();
            }
        }
    };

    /**
     * banner 渲染回调监听
     */
    private TTNativeExpressAd.ExpressAdInteractionListener mExpressAdInteractionListener = new TTNativeExpressAd.ExpressAdInteractionListener() {
        @Override
        public void onAdClicked(View view, int type) {
            if (customEventBannerListener != null) {
                customEventBannerListener.onBannerClicked();
            }
        }

        @Override
        public void onAdShow(View view, int type) {
            if (customEventBannerListener != null) {
                customEventBannerListener.onBannerShow();
            }
        }

        @Override
        public void onRenderFail(View view, String msg, int code) {
            Log.i(ADAPTER_NAME,"RENDER_PROCESS_GONE_UNSPECIFIED");
            if (customEventBannerListener != null) {
                customEventBannerListener.onBannerRenderFailed(msg,code);
            }
        }

        @Override
        public void onRenderSuccess(View view, float width, float height) {
            if (customEventBannerListener != null) {
                //render success add view to mMoPubView
                customEventBannerListener.onBannerRenderSuccess();
            }
        }
    };


}
