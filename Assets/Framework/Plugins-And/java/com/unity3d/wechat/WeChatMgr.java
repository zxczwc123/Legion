package com.unity3d.wechat;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Handler;
import android.util.Log;
import android.widget.Toast;

import com.unity3d.player.R;
import com.tencent.mm.opensdk.constants.ConstantsAPI;
import com.tencent.mm.opensdk.modelbiz.WXLaunchMiniProgram;
import com.tencent.mm.opensdk.modelmsg.SendAuth;
import com.tencent.mm.opensdk.modelmsg.SendMessageToWX;
import com.tencent.mm.opensdk.modelmsg.WXImageObject;
import com.tencent.mm.opensdk.modelmsg.WXMediaMessage;
import com.tencent.mm.opensdk.modelmsg.WXTextObject;
import com.tencent.mm.opensdk.modelmsg.WXWebpageObject;
import com.tencent.mm.opensdk.modelpay.PayReq;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.openapi.IWXAPIEventHandler;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;
import com.unity3d.player.UnityPlayer;

import java.lang.reflect.Field;

public class WeChatMgr {

    private static WeChatMgr instance;

    public static WeChatMgr getInstance() {
        if (instance == null) {
            instance = new WeChatMgr();
        }
        return instance;
    }

    private IWXAPI wxApi;
    private String wechatAppId;
    private static final int THUMB_SIZE = 150;
    private IWeChatListener listener;

    private UnityPlayer unityPlayer;

    public void setListener(IWeChatListener listener){
        this.listener = listener;
    }

    public void setPlayer(UnityPlayer player){
        unityPlayer = player;
    }

    public void init(String wechatAppId) {
        try {
			if(wxApi != null){
				return;
			}
            this.wechatAppId = wechatAppId;
            wxApi = WXAPIFactory.createWXAPI(UnityPlayer.currentActivity, this.wechatAppId);

            //建议动态监听微信启动广播进行注册到微信
            UnityPlayer.currentActivity.registerReceiver(new BroadcastReceiver() {
                @Override
                public void onReceive(Context context, Intent intent) {

                    // 将该app注册到微信
                    wxApi.registerApp(WeChat.wechatAppId);
                }
            }, new IntentFilter(ConstantsAPI.ACTION_REFRESH_WXAPP));
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public boolean isWXAppInstalled() {
        try {
            return wxApi.isWXAppInstalled();
        } catch (Exception e) {
            e.printStackTrace();
            return false;
        }
    }

    public void login() {
        try {
            final SendAuth.Req req = new SendAuth.Req();
            req.scope = "snsapi_userinfo";
            req.state = "login";
            req.transaction = this.buildTransaction("login");
            wxApi.sendReq(req);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public void shareText(String title, String text, int scene) {
        try {
            WXTextObject textObj = new WXTextObject();
            textObj.text = text;

            WXMediaMessage msg = new WXMediaMessage();
            msg.mediaObject = textObj;
            msg.title = title;
            msg.description = text;

            SendMessageToWX.Req req = new SendMessageToWX.Req();
            req.transaction = this.buildTransaction("text");
            req.message = msg;
            req.scene = scene;

            wxApi.sendReq(req);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public void shareUrl(String title, String url, String description, int scene) {
        try {
            WXWebpageObject webpage = new WXWebpageObject();
            webpage.webpageUrl = url;
            WXMediaMessage msg = new WXMediaMessage(webpage);
            msg.title = title;
            Bitmap bitmap = BitmapFactory.decodeResource(UnityPlayer.currentActivity.getResources(), R.mipmap.app_icon);
            msg.setThumbImage(bitmap);
            msg.description = description;
            SendMessageToWX.Req req = new SendMessageToWX.Req();
            req.transaction = buildTransaction("webpage");
            req.message = msg;
            req.scene = scene == 0 ? SendMessageToWX.Req.WXSceneSession : SendMessageToWX.Req.WXSceneTimeline;
            wxApi.sendReq(req);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public void shareImage(String title, String image, String description, int scene) {
        try {
            Bitmap bmp = BitmapFactory.decodeFile(image);
            WXImageObject imgObj = new WXImageObject(bmp);
            WXMediaMessage msg = new WXMediaMessage();
            msg.mediaObject = imgObj;
            msg.title = title;
            msg.description = description;
            Bitmap thumbBmp = Util.createBitmapThumbnail(bmp, true);

            bmp.recycle();
            msg.thumbData = Util.bmpToByteArray(thumbBmp, true);

            SendMessageToWX.Req req = new SendMessageToWX.Req();
            req.transaction = this.buildTransaction("image");
            req.message = msg;
            req.scene = scene;
            wxApi.sendReq(req);
        } catch (Exception ex) {
            Log.v("Unity", ex.getMessage());
            ex.printStackTrace();
        }
    }

    /**
     * 发起支付
     * @param appid
     * @param partnerid
     * @param prepayid
     * @param noncestr
     * @param timestamp
     * @param pack
     * @param sign
     */
    public void pay(String appid ,String partnerid,String prepayid,String noncestr,String timestamp,String pack,String sign){
        try {
            PayReq req = new PayReq();
            //req.appId = "wxf8b4f85f3a794e77";  // 测试appId
            req.appId = appid;
            req.partnerId = partnerid;
            req.prepayId = prepayid;
            req.nonceStr = noncestr;
            req.timeStamp = timestamp;
            req.packageValue = pack;
            req.sign = sign;
            req.extData = "app data"; // optional
            wxApi.sendReq(req);
        }catch  (Exception ex) {
            Log.v("Unity", ex.getMessage());
            ex.printStackTrace();
        }
    }

    public void launchMiniProgram(String username,String path){
        try {
            WXLaunchMiniProgram.Req req = new WXLaunchMiniProgram.Req();
            req.userName = username; // 填小程序原始id
            req.path = path;                  ////拉起小程序页面的可带参路径，不填默认拉起小程序首页，对于小游戏，可以只传入 query 部分，来实现传参效果，如：传入 "?foo=bar"。
            req.miniprogramType = WXLaunchMiniProgram.Req.MINIPTOGRAM_TYPE_RELEASE;// 可选打开 开发版，体验版和正式版
            wxApi.sendReq(req);
        }catch  (Exception ex) {
            Log.v("Unity", ex.getMessage());
            ex.printStackTrace();
        }
    }


    /**
     * 未知原因 一定要运行在 unity 线程不然关闭立马打开的时候会卡很久  出现time out to pause unity
     * @param extraData
     */
    public void onMiniProgramCallback(final String extraData){
        this.runOnUnityThread(new Runnable() {
            @Override
            public void run() {
                if(WeChatMgr.this.listener != null) WeChatMgr.this.listener.onMiniProgramCallback(extraData);
            }
        });
    }

    public void onLoginCallback(final String code){
        Log.i("wechat", code);
        this.runOnUnityThread(new Runnable() {
            @Override
            public void run() {
                if(WeChatMgr.this.listener != null) WeChatMgr.this.listener.onLoginCallback(code);
            }
        });
    }

    public void onShareCallback(final int code){

        this.runOnUnityThread(new Runnable() {
            @Override
            public void run() {
                if(WeChatMgr.this.listener != null) WeChatMgr.this.listener.onShareCallback(code);
            }
        });
    }

    public void onPayCallback(final int code,final String orderId){

        this.runOnUnityThread(new Runnable() {
            @Override
            public void run() {
                if(WeChatMgr.this.listener != null) WeChatMgr.this.listener.onPayCallback(code,orderId);
            }
        });
    }

    private String buildTransaction(final String type) {
        return (type == null) ? String.valueOf(System.currentTimeMillis()) : type + System.currentTimeMillis();
    }

    private Handler unityThreadHandler;

    private void runOnUnityThread(Runnable runnable){
        try{
            if(unityThreadHandler == null){
                Field field = unityPlayer.getClass().getDeclaredField("a");
                field.setAccessible(true);
                Thread thread = ((Thread) field.get(unityPlayer));
                Field fieldHandler = thread.getClass().getDeclaredField("a");
                fieldHandler.setAccessible(true);
                Handler handler = (Handler)fieldHandler.get(thread);
                unityThreadHandler = handler;
            }
            unityThreadHandler.post(runnable);
        }catch (Exception e){
            e.printStackTrace();
        }
    }
}