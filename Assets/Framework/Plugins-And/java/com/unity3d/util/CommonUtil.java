package com.unity3d.util;

import android.Manifest;
import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.net.Uri;
import android.os.Build;
import android.support.v4.app.ActivityCompat;
import android.telephony.TelephonyManager;
import android.util.Log;
import android.widget.Toast;

import com.bun.miitmdid.core.MdidSdkHelper;
import com.bun.miitmdid.interfaces.IIdentifierListener;
import com.bun.miitmdid.interfaces.IdSupplier;
import com.unity3d.player.UnityPlayer;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;

public class CommonUtil {

    /**
     * 打开qq聊天
     *
     * @param qq
     */
    public static void openQQChat(String qq) {
        try {
            String url = "mqqwpa://im/chat?chat_type=wpa&uin=" + qq;
            UnityPlayer.currentActivity.startActivity(new Intent(Intent.ACTION_VIEW, Uri.parse(url)));
        } catch (Exception e) {
            Toast.makeText(UnityPlayer.currentActivity, "尚未安装QQ", Toast.LENGTH_SHORT).show();
        }
    }

    public static void copyTextToClipboard(String paramString) {
        Log.i("CommonUtil", "copyTextToClipboard");
        try {
            ClipboardManager clipboard = (ClipboardManager) UnityPlayer.currentActivity.getSystemService(Context.CLIPBOARD_SERVICE);
            ClipData data = ClipData.newPlainText("data", paramString);
            clipboard.setPrimaryClip(data);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static void getDeviceId(IGetDeviceIdListener listener) {
        if (Build.VERSION.SDK_INT >= 29) {
            GetOAID( listener);
        } else {
            try{
                TelephonyManager telephonyManager = (TelephonyManager) UnityPlayer.currentActivity.getSystemService(Context.TELEPHONY_SERVICE);
                String deviceId = telephonyManager.getDeviceId();
                if(listener != null){
                    listener.onDeviceIdCallback(deviceId);
                }
            }catch (Exception e){
                e.printStackTrace();
                if(listener != null){
                    listener.onDeviceIdCallback("");
                }
            }
        }
    }

    public static String getDeviceModel() {
        Log.d("CommonUtil", "Build.MODEL===" + Build.MODEL);
        return Build.MODEL;
    }

    public static void GetOAID(final IGetDeviceIdListener listener) {
        long timeb = System.currentTimeMillis();
        MdidSdkHelper.InitSdk(UnityPlayer.currentActivity.getApplicationContext(), true, new IIdentifierListener()
        {
            public void OnSupport(boolean paramAnonymousBoolean, IdSupplier paramAnonymousIdSupplier)
            {
                if (paramAnonymousIdSupplier == null) {
                    return;
                }
                String oaid = paramAnonymousIdSupplier.getOAID();
                if(listener != null){
                    listener.onDeviceIdCallback(oaid);
                }
            }
        });
    }

    private final static String[] strHex = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f"};

    public static String getMD5FromAssets(String filename) {
        StringBuffer sb = new StringBuffer();
        try {
            MessageDigest md = MessageDigest.getInstance("MD5");
            byte[] b = md.digest(readFileBytes(filename));
            for (int i = 0; i < b.length; i++) {
                int d = b[i];
                if (d < 0) {
                    d += 256;
                }
                int d1 = d / 16;
                int d2 = d % 16;
                sb.append(strHex[d1] + strHex[d2]);
            }
        } catch (NoSuchAlgorithmException e) {
            e.printStackTrace();
        } catch (Exception e) {
            e.printStackTrace();
        }
        return sb.toString();
    }

    private static byte[] readFileBytes(String fileName) {

        try {
//得到资源中的Raw数据流
            InputStream in = UnityPlayer.currentActivity.getResources().getAssets().open(fileName);
//得到数据的大小
            int length = in.available();
            byte[] buffer = new byte[length];
//读取数据
            in.read(buffer);
//依test.txt的编码类型选择合适的编码，如果不调整会乱码
// res = EncodingUtils.getString(buffer, "BIG5");
//关闭
            in.close();
            return buffer;
        } catch (Exception e) {
            e.printStackTrace();
            return null;
        }
    }

    
}