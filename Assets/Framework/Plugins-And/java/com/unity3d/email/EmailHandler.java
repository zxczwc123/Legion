package com.unity3d.email;

import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.widget.Toast;

import com.unity3d.player.UnityPlayerActivity;

/**
 * Created by zxc on 2019/6/28.
 */

public class EmailHandler {

    public static Context context;

    /**
     * 显示splash
     */
    public static void openEmail(String uri)
    {
        try {
            Intent data=new Intent(Intent.ACTION_SENDTO);
            data.setData(Uri.parse("mailto:"+uri));
            data.putExtra(Intent.EXTRA_SUBJECT, "");
            data.putExtra(Intent.EXTRA_TEXT, "");
            context.startActivity(Intent.createChooser(data, "Select email application."));
        } catch (Exception e) {
            e.printStackTrace();
            Toast.makeText(context, "", Toast.LENGTH_LONG).show();
        }

//        try {
//            String[] email = {uri}; // 需要注意，email必须以数组形式传入
//            Intent intent = new Intent(Intent.ACTION_SEND);
//            intent.setType("message/rfc882");
//            intent.putExtra(Intent.EXTRA_EMAIL, email); // 接收人
//            intent.putExtra(Intent.EXTRA_CC, email); // 抄送人
//            intent.putExtra(Intent.EXTRA_BCC, email);
//            intent.putExtra(Intent.EXTRA_SUBJECT, ""); // 主题
//            intent.putExtra(Intent.EXTRA_TEXT, ""); // 正文
//            Native.getContext().startActivity(Intent.createChooser(intent, "Select email application."));
//        } catch (Exception e) {
//            e.printStackTrace();
//            Toast.makeText(Native.getContext(), "", Toast.LENGTH_LONG).show();
//        }
    }
}
