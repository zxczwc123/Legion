package com.unity3d.splash;

import android.app.Activity;
import android.media.Image;
import android.os.Handler;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.ImageView;

import com.unity3d.player.R;
import com.unity3d.player.UnityPlayer;

/**
 * Created by zxc on 2019/6/28.
 */

public class SplashHandler {

    private static ImageView splashView;

    private static UnityPlayer unityPlayer;

    /**
     * 显示splash
     */
    public static void showSplash(UnityPlayer player)
    {
        if(player == null){
            return;
        }
        unityPlayer = player;

        splashView = new ImageView(UnityPlayer.currentActivity);
        unityPlayer.addView(splashView);
        splashView.setImageResource(R.drawable.splash);
        splashView.setScaleType(ImageView.ScaleType.CENTER_CROP);

        // 倒计时隐藏splash
        Handler handler = new Handler();
        handler.postDelayed(new Runnable() {
            @Override
            public void run() {
                hideSplash();
            }
        },10000);
    }

    /**
     * 由Unity调用
     * 隐藏splash
     */
    public static void hideSplash() {
        unityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (unityPlayer == null) {
                    return;
                }
                if (splashView != null) {
                    unityPlayer.removeView(splashView);
                    splashView = null;
                    unityPlayer = null;
                }
            }
        });
    }
}
