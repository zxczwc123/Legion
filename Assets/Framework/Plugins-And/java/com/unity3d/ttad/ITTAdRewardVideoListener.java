package com.unity3d.ttad;

public interface ITTAdRewardVideoListener {

    void onRewardVideoLoadFailed(int code,String message);

    void onRewardVideoLoaded();

    void onRewardVideoShow();

    void onRewardVideoBarClick();

    void onRewardVideoClose();

    void onRewardVideoComplete();

    void onRewardVideoError();

    void onRewardVideoRewardVertify(boolean rewardVerify, int rewardAmount, String rewardName, int errorCode, String errorMsg);

    void onRewardVideoSkipped();
}
