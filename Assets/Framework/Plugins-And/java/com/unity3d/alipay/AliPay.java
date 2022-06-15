package com.unity3d.alipay;

import android.app.Activity;

import com.unity3d.player.UnityPlayer;

public class AliPay {

    public static void setListener(IAliPayListener listener){
        AliPayMgr.getInstance().setListener(listener);
    }

    public static void init(){
        Activity app = UnityPlayer.currentActivity;
        app.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                AliPayMgr.getInstance();
            }
        });
    }

    public static  boolean isAliAppInstalled(){
        return false;
    }

    public static void pay(final String orderInfo){
        Activity app = UnityPlayer.currentActivity;
        app.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                AliPayMgr.getInstance().payV2(orderInfo);
            }
        });

    }

    public static String getSdkVersion(){
        return AliPayMgr.getInstance().getSdkVersion();
    }

    public static void h5Pay(final String url){
        Activity app = UnityPlayer.currentActivity;
        app.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                AliPayMgr.getInstance().h5Pay(url);
            }
        });

    }

    public static void auth(final String info){
        Activity app = UnityPlayer.currentActivity;
        app.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                AliPayMgr.getInstance().authV2(info);
            }
        });
    }
}
