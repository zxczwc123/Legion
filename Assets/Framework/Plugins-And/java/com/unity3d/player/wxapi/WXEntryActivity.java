package com.unity3d.player.wxapi;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;

import com.hf.wechat.WeChat;
import com.hf.wechat.WeChatMgr;
import com.tencent.mm.opensdk.constants.ConstantsAPI;
import com.tencent.mm.opensdk.modelbase.BaseReq;
import com.tencent.mm.opensdk.modelbase.BaseResp;
import com.tencent.mm.opensdk.modelbiz.SubscribeMessage;
import com.tencent.mm.opensdk.modelbiz.WXLaunchMiniProgram;
import com.tencent.mm.opensdk.modelbiz.WXOpenBusinessView;
import com.tencent.mm.opensdk.modelbiz.WXOpenBusinessWebview;
import com.tencent.mm.opensdk.modelmsg.SendAuth;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.openapi.IWXAPIEventHandler;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;
import com.unity3d.player.UnityPlayer;


public class WXEntryActivity extends Activity implements IWXAPIEventHandler {

    private static String TAG = "MicroMsg.WXEntryActivity";
    private IWXAPI api;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        api = WXAPIFactory.createWXAPI(this, WeChat.wechatAppId, false);
        try {
            Intent intent = getIntent();
            api.handleIntent(intent, this);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);

        setIntent(intent);
        api.handleIntent(intent, this);
    }

    @Override
    public void onReq(BaseReq req) {
        switch (req.getType()) {
            case ConstantsAPI.COMMAND_GETMESSAGE_FROM_WX:
            case ConstantsAPI.COMMAND_SHOWMESSAGE_FROM_WX:
            default:
                break;
        }
        finish();
    }

    @Override
    public void onResp(BaseResp resp) {
        switch (resp.errCode) {
            case BaseResp.ErrCode.ERR_OK:
            case BaseResp.ErrCode.ERR_USER_CANCEL:
            case BaseResp.ErrCode.ERR_AUTH_DENIED:
            case BaseResp.ErrCode.ERR_UNSUPPORT:
            default:
                break;
        }

        if (resp.getType() == ConstantsAPI.COMMAND_SUBSCRIBE_MESSAGE) {
            SubscribeMessage.Resp subscribeMsgResp = (SubscribeMessage.Resp) resp;
        }

        if (resp.getType() == ConstantsAPI.COMMAND_LAUNCH_WX_MINIPROGRAM) {
            WXLaunchMiniProgram.Resp launchMiniProgramResp = (WXLaunchMiniProgram.Resp) resp;
            String extraData = launchMiniProgramResp.extMsg;
            // 回调
            WeChatMgr.getInstance().onMiniProgramCallback(extraData);
        }

        if (resp.getType() == ConstantsAPI.COMMAND_OPEN_BUSINESS_VIEW) {
            WXOpenBusinessView.Resp launchMiniProgramResp = (WXOpenBusinessView.Resp) resp;
        }

        if (resp.getType() == ConstantsAPI.COMMAND_OPEN_BUSINESS_WEBVIEW) {
            WXOpenBusinessWebview.Resp response = (WXOpenBusinessWebview.Resp) resp;
        }

        if (resp.getType() == ConstantsAPI.COMMAND_SENDAUTH) {
        	// 登陆回调
            final String code;
            if (resp.errCode == BaseResp.ErrCode.ERR_OK) {
                SendAuth.Resp authResp = (SendAuth.Resp) resp;
                code = authResp.code;
            } else {
                code = "";
            }
            // 回调
            WeChatMgr.getInstance().onLoginCallback(code);
        } else if(resp.getType() == ConstantsAPI.COMMAND_SUBSCRIBE_MESSAGE) {
        	// 分享回到
            final int code = resp.errCode;
            // 回调
            WeChatMgr.getInstance().onShareCallback(code);
        }
        finish();
    }
}