package com.unity3d.alipay;

public interface IAliPayListener {

    void onPayCallback(int code,String data);

    void onAuthCallback(int code,String data);
}
