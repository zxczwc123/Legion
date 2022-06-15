package com.unity3d.ttad;

public interface ITTAdInterstitialListener {
    void onInterstitialLoadFailed(int code,String message);

    void onInterstitialLoaded();

    void onInterstitialDismissed();

    void onInterstitialClicked();

    void onInterstitialShow();

    void onInterstitialRenderFailed(String message,int code);

    void onInterstitialRenderSuccess();
}
