using System;
using System.IO;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 资源文件导入自动设置程序 
/// GaoHui
/// </summary>
public class AssetProcessor : AssetPostprocessor
{
    [MenuItem("Assets/Custom Reimport Images")]
    public static void SetAllTextureType()
    {
        //获取鼠标点击图片目录
        var arr = Selection.GetFiltered(typeof(DefaultAsset), SelectionMode.Assets);
        string folder = AssetDatabase.GetAssetPath(arr[0]);
        Debug.Log("Reimport Path:" + folder);

        //针对目录下的所有文件进行遍历 取出.png和.jpg文件进行处理 可自行拓展
        DirectoryInfo direction = new DirectoryInfo(folder);
        FileInfo[] pngFiles = direction.GetFiles("*.png", SearchOption.AllDirectories);
        FileInfo[] jpgfiles = direction.GetFiles("*.jpg", SearchOption.AllDirectories);

        try
        {
            SetTexture(pngFiles);
            SetTexture(jpgfiles);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }

    static void SetTexture(FileInfo[] fileInfo)
    {
        for (int i = 0; i < fileInfo.Length; i++)
        {
            //这里第一次写时有一个接口可直接调用，但是第二次写时找不到了 所以用了切割字符
            string fullpath = fileInfo[i].FullName.Replace("\\", "/");
            string path = fullpath.Replace(Application.dataPath, "Assets");
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            EditorUtility.DisplayProgressBar("批量处理图片", fileInfo[i].Name, i / (float)fileInfo.Length);
            SetTextureFormat(textureImporter);
        }
    }

    //设置图片格式
    static void SetTextureFormat(TextureImporter textureImporter)
    {
        textureImporter.mipmapEnabled = false;
        textureImporter.isReadable = false;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.wrapMode = TextureWrapMode.Clamp;
        textureImporter.npotScale = TextureImporterNPOTScale.None;
        textureImporter.textureFormat = TextureImporterFormat.Automatic;
        textureImporter.textureCompression = TextureImporterCompression.Compressed;


        //IOS端单独设置
        TextureImporterPlatformSettings setting_iphone = new TextureImporterPlatformSettings();
        setting_iphone.overridden = true;
        setting_iphone.name = "iPhone";
        //根据是否有透明度，选择RGBA还是RGB
        if (textureImporter.DoesSourceTextureHaveAlpha())
            setting_iphone.format = TextureImporterFormat.ASTC_4x4;
        else
            setting_iphone.format = TextureImporterFormat.ASTC_4x4;
        textureImporter.SetPlatformTextureSettings(setting_iphone);
    }
}    

