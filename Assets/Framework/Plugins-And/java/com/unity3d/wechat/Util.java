package com.unity3d.wechat;

import android.graphics.Bitmap;
import android.graphics.Bitmap.CompressFormat;
import android.graphics.Matrix;
import android.util.Log;

import org.json.JSONObject;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.io.RandomAccessFile;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLConnection;

public class Util {

    private static final String TAG = "SDK_Sample.Util";

    public static byte[] bmpToByteArray(final Bitmap bmp, final boolean needRecycle) {
        ByteArrayOutputStream output = new ByteArrayOutputStream();
        bmp.compress(CompressFormat.PNG, 100, output);
        if (needRecycle && !bmp.isRecycled()) {
            bmp.recycle();
        }

        byte[] result = output.toByteArray();
        try {
            output.close();
        } catch (Exception e) {
            e.printStackTrace();
        }

        return result;
    }

    public static byte[] getHtmlByteArray(final String url) {
        URL htmlUrl = null;
        InputStream inStream = null;
        try {
            htmlUrl = new URL(url);
            URLConnection connection = htmlUrl.openConnection();
            HttpURLConnection httpConnection = (HttpURLConnection) connection;
            int responseCode = httpConnection.getResponseCode();
            if (responseCode == HttpURLConnection.HTTP_OK) {
                inStream = httpConnection.getInputStream();
            }
        } catch (MalformedURLException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
        byte[] data = inputStreamToByte(inStream);

        return data;
    }

    public static byte[] inputStreamToByte(InputStream is) {
        try {
            ByteArrayOutputStream bytestream = new ByteArrayOutputStream();
            int ch;
            while ((ch = is.read()) != -1) {
                bytestream.write(ch);
            }
            byte imgdata[] = bytestream.toByteArray();
            bytestream.close();
            return imgdata;
        } catch (Exception e) {
            e.printStackTrace();
        }

        return null;
    }

    public static Bitmap createBitmapThumbnail(Bitmap bitMap, boolean needRecycle) {
        int width = bitMap.getWidth();
        int height = bitMap.getHeight();
        // 设置想要的大小
        int newWidth = 80;
        int newHeight = 80;
        // 计算缩放比例
        float scaleWidth = ((float) newWidth) / width;
        float scaleHeight = ((float) newHeight) / height;
        // 取得想要缩放的matrix参数
        Matrix matrix = new Matrix();
        matrix.postScale(scaleWidth, scaleHeight);
        // 得到新的图片
        Bitmap newBitMap = Bitmap.createBitmap(bitMap, 0, 0, width, height,
                matrix, true);
        if (needRecycle) bitMap.recycle();
        return newBitMap;
    }

    public static Bitmap ImageCompress(Bitmap bitmap) {
        // 图片允许最大空间 单位：KB
        double maxSize = 32.00;
        // 将bitmap放至数组中，意在bitmap的大小（与实际读取的原文件要大）
        ByteArrayOutputStream baos = new ByteArrayOutputStream();
        bitmap.compress(CompressFormat.JPEG, 100, baos);
        byte[] b = baos.toByteArray();
        // 将字节换成KB
        double mid = b.length / 1024;
        // 判断bitmap占用空间是否大于允许最大空间 如果大于则压缩 小于则不压缩
        if (mid > maxSize) {
            // 获取bitmap大小 是允许最大大小的多少倍
            double i = mid / maxSize;
            // 开始压缩 此处用到平方根 将宽带和高度压缩掉对应的平方根倍
            bitmap = zoomImage(bitmap, bitmap.getWidth() / Math.sqrt(i),
                    bitmap.getHeight() / Math.sqrt(i));
        }
        return bitmap;
    }

    /***
     * 图片压缩方法二
     *
     * @param bgimage
     *            ：源图片资源
     * @param newWidth
     *            ：缩放后宽度
     * @param newHeight
     *            ：缩放后高度
     * @return
     */
    public static Bitmap zoomImage(Bitmap bgimage, double newWidth, double newHeight) {
        // 获取这个图片的宽和高
        float width = bgimage.getWidth();
        float height = bgimage.getHeight();
        // 创建操作图片用的matrix对象
        Matrix matrix = new Matrix();
        // 计算宽高缩放率
        float scaleWidth = ((float) newWidth) / width;
        float scaleHeight = ((float) newHeight) / height;
        // 缩放图片动作
        matrix.postScale(scaleWidth, scaleHeight);
        Bitmap bitmap = Bitmap.createBitmap(bgimage, 0, 0, (int) width,
                (int) height, matrix, true);
        return bitmap;
    }

    public static byte[] readFromFile(String fileName, int offset, int len) {
        if (fileName == null) {
            return null;
        }

        File file = new File(fileName);
        if (!file.exists()) {
            Log.i(TAG, "readFromFile: file not found");
            return null;
        }

        if (len == -1) {
            len = (int) file.length();
        }

        Log.d(TAG, "readFromFile : offset = " + offset + " len = " + len + " offset + len = " + (offset + len));

        if (offset < 0) {
            Log.e(TAG, "readFromFile invalid offset:" + offset);
            return null;
        }
        if (len <= 0) {
            Log.e(TAG, "readFromFile invalid len:" + len);
            return null;
        }
        if (offset + len > (int) file.length()) {
            Log.e(TAG, "readFromFile invalid file len:" + file.length());
            return null;
        }

        byte[] b = null;
        try {
            RandomAccessFile in = new RandomAccessFile(fileName, "r");
            b = new byte[len]; // ��寤哄���?���浠跺ぇ灏����扮�
            in.seek(offset);
            in.readFully(b);
            in.close();

        } catch (Exception e) {
            Log.e(TAG, "readFromFile : errMsg = " + e.getMessage());
            e.printStackTrace();
        }
        return b;
    }

    private static final int MAX_DECODE_PICTURE_SIZE = 1920 * 1440;

    //    public static Bitmap extractThumbNail(final String path, final int height, final int width, final boolean crop) {
//        Assert.assertTrue(path != null && !path.equals("") && height > 0 && width > 0);
//
//        BitmapFactory.Options options = new BitmapFactory.Options();
//
//        try {
//            options.inJustDecodeBounds = true;
//            Bitmap tmp = BitmapFactory.decodeFile(path, options);
//            if (tmp != null&&!tmp.isRecycled()) {
//                tmp.recycle();
//                tmp = null;
//            }
//
//            Log.d(TAG, "extractThumbNail: round=" + width + "x" + height + ", crop=" + crop);
//            final double beY = options.outHeight * 1.0 / height;
//            final double beX = options.outWidth * 1.0 / width;
//            Log.d(TAG, "extractThumbNail: extract beX = " + beX + ", beY = " + beY);
//            options.inSampleSize = (int) (crop ? (beY > beX ? beX : beY) : (beY < beX ? beX : beY));
//            if (options.inSampleSize <= 1) {
//                options.inSampleSize = 1;
//            }
//
//            // NOTE: out of memory error
//            while (options.outHeight * options.outWidth / options.inSampleSize > MAX_DECODE_PICTURE_SIZE) {
//                options.inSampleSize++;
//            }
//
//            int newHeight = height;
//            int newWidth = width;
//            if (crop) {
//                if (beY > beX) {
//                    newHeight = (int) (newWidth * 1.0 * options.outHeight / options.outWidth);
//                } else {
//                    newWidth = (int) (newHeight * 1.0 * options.outWidth / options.outHeight);
//                }
//            } else {
//                if (beY < beX) {
//                    newHeight = (int) (newWidth * 1.0 * options.outHeight / options.outWidth);
//                } else {
//                    newWidth = (int) (newHeight * 1.0 * options.outWidth / options.outHeight);
//                }
//            }
//
//            options.inJustDecodeBounds = false;
//
//            Log.i(TAG, "bitmap required size=" + newWidth + "x" + newHeight + ", orig=" + options.outWidth + "x" + options.outHeight + ", sample=" + options.inSampleSize);
//            Bitmap bm = BitmapFactory.decodeFile(path, options);
//            if (bm == null) {
//                Log.e(TAG, "bitmap decode failed");
//                return null;
//            }
//
//            Log.i(TAG, "bitmap decoded size=" + bm.getWidth() + "x" + bm.getHeight());
//            final Bitmap scale = Bitmap.createScaledBitmap(bm, newWidth, newHeight, true);
//            if (scale != null && ! bm.isRecycled()) {
//                bm.recycle();
//                bm = scale;
//            }
//
//            if (crop) {
//                final Bitmap cropped = Bitmap.createBitmap(bm, (bm.getWidth() - width) >> 1, (bm.getHeight() - height) >> 1, width, height);
//                if (cropped == null) {
//                    return bm;
//                }
//
//                if(!bm.isRecycled())
//                    bm.recycle();
//                bm = cropped;
//                Log.i(TAG, "bitmap croped size=" + bm.getWidth() + "x" + bm.getHeight());
//            }
//            return bm;
//
//        } catch (final OutOfMemoryError e) {
//            Log.e(TAG, "decode bitmap failed: " + e.getMessage());
//            options = null;
//        }
//
//        return null;
//    }

    public static void JSONPut(JSONObject json, final String key, final Object value) {
        try {
            json.put(key, value);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}

