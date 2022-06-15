package com.unity3d.wechat;

public class WeChat {

    public static String wechatAppId = "";

    public static void setListener(IWeChatListener listener){
        WeChatMgr.getInstance().setListener(listener);
    }

    public static void init() {
        WeChatMgr.getInstance().init(wechatAppId);
    }

    public static boolean isWXAppInstalled() {
        return WeChatMgr.getInstance().isWXAppInstalled();
    }

    public static void login() {
        WeChatMgr.getInstance().login();
    }

    public static void shareText(String title, String text, int scene) {
        WeChatMgr.getInstance().shareText(title, text, scene);
    }

    public static void shareUrl(String title, String url, String description, int scene) {
        WeChatMgr.getInstance().shareUrl(title, url, description, scene);
    }

    public static void shareImage(String title, String image, String description, int scene) {
        WeChatMgr.getInstance().shareImage(title, image, description, scene);
    }

    public static void pay(String appid,String partnerid,String prepayid,String noncestr,String timestamp,String pack,String sign){
        WeChatMgr.getInstance().pay( appid, partnerid, prepayid, noncestr, timestamp, pack, sign);
    }

    public static void launchMiniProgram(String username,String path){
        WeChatMgr.getInstance().launchMiniProgram(username,path);
    }
}