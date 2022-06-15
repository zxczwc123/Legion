package com.unity3d.util;

import android.content.Intent;
import android.net.Uri;
import android.text.TextUtils;
import android.widget.Toast;

import com.unity3d.player.UnityPlayer;

/**
 * Created by zxc on 2019/6/30.
 */

public class StoreUtil {

    private final static String GOOGLE_PLAY = "com.android.vending";//这里对应的是谷歌商店，跳转别的商店改成对应的即可

    /**
     * 打开商城 对应的应用详情界面
     */
    public static boolean openStore(String packageName){
        try {
            if (TextUtils.isEmpty(packageName))
                return false;
            Uri uri = Uri.parse("market://details?id=" + packageName);
            Intent intent = new Intent(Intent.ACTION_VIEW, uri);
            intent.setPackage(GOOGLE_PLAY);
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            if (intent.resolveActivity(UnityPlayer.currentActivity.getPackageManager()) != null) { //有浏览器
                UnityPlayer.currentActivity.startActivity(intent);
            } else {
                return false;
            }
        } catch (Exception e) {
            e.printStackTrace();
            return false;
        }
        return true;
    }

    /**
     * 打开商城 并搜索关键字
     */
    public static boolean searchStore(String key){
        try {
            if (TextUtils.isEmpty(key))
                return false;
            Uri uri = Uri.parse("market://search?q=" + key);
            Intent intent = new Intent(Intent.ACTION_VIEW, uri);
            intent.setPackage(GOOGLE_PLAY);
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            if (intent.resolveActivity(UnityPlayer.currentActivity.getPackageManager()) != null) { //有浏览器
                UnityPlayer.currentActivity.startActivity(intent);
            } else {
                return false;
            }
        } catch (Exception e) {
            e.printStackTrace();
            return false;
        }
        return true;
    }

    /**
     * 评价app
     */
    public static void rateApp(int score){

    }
}
