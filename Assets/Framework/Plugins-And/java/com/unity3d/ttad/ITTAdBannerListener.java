package com.unity3d.ttad;

public interface ITTAdBannerListener {
    void onBannerFailed(int code,String msg);

    void onBannerLoaded();

    void onBannerClicked();

    void onBannerShow();

    void onBannerRenderFailed(String msg,int code);

    void onBannerRenderSuccess();
}
