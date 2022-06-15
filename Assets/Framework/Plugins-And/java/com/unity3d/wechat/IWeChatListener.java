package com.unity3d.wechat;

public interface IWeChatListener {

    void onLoginCallback(String code);

    void onShareCallback(int code);

    void onPayCallback(int code,String orderId);

    void onMiniProgramCallback(String extraData);
}
