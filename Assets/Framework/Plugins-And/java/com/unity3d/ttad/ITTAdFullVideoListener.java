package com.unity3d.ttad;

public interface ITTAdFullVideoListener {

    void onFullVideoLoadFailed(int code,String message);

    void onFullVideoLoaded();

    void onFullVideoShow();

    void onFullVideoAdVideoBarClick();

    void onFullVideoAdClose();

    void onFullVideoComplete();

    void onFullVideoSkipped();

}
