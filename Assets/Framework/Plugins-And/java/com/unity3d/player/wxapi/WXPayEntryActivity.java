package com.unity3d.player.wxapi;

import com.hf.wechat.WeChat;
import com.hf.wechat.WeChatMgr;
import com.tencent.mm.opensdk.constants.ConstantsAPI;
import com.tencent.mm.opensdk.modelbase.BaseReq;
import com.tencent.mm.opensdk.modelbase.BaseResp;
import com.tencent.mm.opensdk.modelbiz.WXOpenBusinessView;
import com.tencent.mm.opensdk.modelbiz.WXPayInsurance;
import com.tencent.mm.opensdk.modelpay.PayResp;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.openapi.IWXAPIEventHandler;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;
import com.unity3d.player.UnityPlayer;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

public class WXPayEntryActivity extends Activity implements IWXAPIEventHandler{
	
	private static final String TAG = "MicroMsg.SDKSample.WXPayEntryActivity";
	
    private IWXAPI api;
	
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

    	api = WXAPIFactory.createWXAPI(this, WeChat.wechatAppId);
        api.handleIntent(getIntent(), this);
    }

	@Override
	protected void onNewIntent(Intent intent) {
		super.onNewIntent(intent);
		setIntent(intent);
        api.handleIntent(intent, this);
	}

	@Override
	public void onReq(BaseReq req) {
	}

	@Override
	public void onResp(BaseResp resp) {
		Log.d("Unity", "onPayFinish, errCode = " + resp.errCode);

		if (resp.getType() == ConstantsAPI.COMMAND_PAY_BY_WX) {

			AlertDialog.Builder builder = new AlertDialog.Builder(UnityPlayer.currentActivity);
			builder.setTitle("微信支付");
			if(resp.errCode == BaseResp.ErrCode.ERR_OK){
				builder.setMessage("支付成功");
				PayResp payResp = (PayResp) resp;

				WeChatMgr.getInstance().onPayCallback(resp.errCode, payResp.prepayId);
			}else{
				builder.setMessage("支付失败");

				WeChatMgr.getInstance().onPayCallback(resp.errCode, "");
			}
			builder.show();
		}

		finish();
	}
}