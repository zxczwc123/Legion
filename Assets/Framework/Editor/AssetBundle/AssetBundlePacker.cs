// ========================================================
// 描 述：AssetBundlePacker.cs 
// 作 者：郑贤春 
// 时 间：2017/02/24 15:19:22 
// 版 本：5.4.1f1 
// ========================================================
using Framework.Core;
using Framework.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework.MEditor.AssetBundleEditor
{
    /// <summary>
    /// 此打包只打包所有资源
    /// </summary>
    public class AssetBundlePacker : Editor
    {

        /// <summary>
        /// 设置选择的文件夹下的所有资源，并进行打包
        /// </summary>
        //[MenuItem("Assets/资源工具/打包AB资源", false, 0)]
        public static void PackAssetBundle()
        {
            // 选中文件夹 即选中文件夹下所有文件包括文件夹
            UnityEngine.Object[] objs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            AssetBundlePacker.AutoPackObjects(objs);
        }

        [MenuItem("Assets/资源工具/清除AB配置", false, 0)]
        public static void ClearAssetBundleSetting() {
            // 选中文件夹 即选中文件夹下所有文件包括文件夹
            UnityEngine.Object[] objs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            for (int i = 0; i < objs.Length; i++) {
                UnityEngine.Object obj = objs[i];
                // assetPath 格式 Assets/Game/audios
                string assetPath = AssetDatabase.GetAssetPath(obj);

                if (Directory.Exists(assetPath)) {
                    // 是文件夹资源继续
                    continue;
                }
                var extension = Path.GetExtension(assetPath);
                if (extension == ".cs") {
                    // 是代码继续
                    continue;
                }
                // 路径下 带有Prefabs的 .prefab的资源自己打包一个ab 
                // 非.prefab的资源 按文件夹打包 所以其他assetbundle 以文件夹为名 prefab以自己为名

                var importer = AssetImporter.GetAtPath(assetPath);
                importer.assetBundleName = null;
            }
        }

        /// <summary>
        /// 打包 /Game/Res下的资源
        /// </summary>
        //[MenuItem("Assets/资源工具/打包AB资源", false, 0)]
        public static void GenerateAssetBundle() {
            var resPath = Path.Combine(Application.dataPath ,PathUtility.ResPath);
            var resPathAllFiles = Directory.GetFiles(resPath, "*.*", SearchOption.AllDirectories).ToList();
            var resFiles = resPathAllFiles.FindAll(f => {
                var _f = f.ToLower();
                var exten = Path.GetExtension(_f);
                if (exten.Equals(".meta") || exten.Equals(".cs")) {
                    return false;
                }
                return true;
            });

            for (int i = 0; i < resFiles.Count; i++) {
                string fileName = resFiles[i];
                if (Directory.Exists(fileName)) {
                    // 是文件夹资源继续
                    continue;
                }
                // 路径下 带有Prefabs的 .prefab的资源自己打包一个ab 
                // 非.prefab的资源 按文件夹打包 所以其他assetbundle 以文件夹为名 prefab以自己为名
                var assetPath = fileName.Substring(fileName.IndexOf("Assets"));
                var importer = AssetImporter.GetAtPath(assetPath);
                if (importer == null) continue;
                importer.assetBundleName = GetAssetBundleName(assetPath);
            }

            BuildTarget buildTarget = BuildTarget.Android;
#if UNITY_IOS
            buildTarget = BuildTarget.iOS;
#elif UNITY_ANDROID
            buildTarget = BuildTarget.Android;
#else 
            buildTarget = BuildTarget.StandaloneWindows;
#endif
            var outPath = PathUtility.GetStreamingAssetPlatformPath();
            if (!Directory.Exists(outPath)) {
                Directory.CreateDirectory(outPath);
            }
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.None, buildTarget);
            AssetDatabase.Refresh();
            Debug.Log("打包资源完毕！");
        }

        /// <summary>
        /// .prefab的资源自己打包一个ab 
        /// 非.prefab的资源 按文件夹打包 所以其他assetBundle 以文件夹为名 prefab以自己为名
        /// 打包后的资源目录会以小写的方式 
        /// </summary>
        /// <param name="objs"></param>
        private static void AutoPackObjects(UnityEngine.Object[] objs)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                UnityEngine.Object obj = objs[i];
                // assetPath 格式 Assets/Game/audios
                string assetPath = AssetDatabase.GetAssetPath(obj);
                
                if(Directory.Exists(assetPath)){
                    // 是文件夹资源继续
                    continue;
                }
                var extension  = Path.GetExtension(assetPath);
                if(extension == ".cs"){
                    // 是代码继续
                    continue;
                }
                // 路径下 带有Prefabs的 .prefab的资源自己打包一个ab 
                // 非.prefab的资源 按文件夹打包 所以其他assetbundle 以文件夹为名 prefab以自己为名

                var importer = AssetImporter.GetAtPath(assetPath);
                importer.assetBundleName = GetAssetBundleName(assetPath);
            }

            BuildTarget buildTarget = BuildTarget.Android;
#if UNITY_IOS
            buildTarget = BuildTarget.iOS;
#elif UNITY_ANDROID
            buildTarget = BuildTarget.Android;
#else 
            buildTarget = BuildTarget.StandaloneWindows;
#endif
            var outPath = PathUtility.GetStreamingAssetPlatformPath();
            if(!Directory.Exists(outPath)){
                Directory.CreateDirectory(outPath);
            }
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.None, buildTarget);
            AssetDatabase.Refresh();
            Debug.Log("打包资源完毕！");
        }

        /// <summary>
        /// 获取AssetBundle 包名
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        private static string GetAssetBundleName(string assetPath){
            // 如果路径中包含Resources去除Resources之前部分
            if (assetPath.Contains("Resources")) {
                var index = assetPath.IndexOf("Resources", StringComparison.Ordinal);
                if (index != -1) {
                    assetPath = assetPath.Substring(index + "Resources/".Length);
                }
            }else if (assetPath.Contains(PathUtility.ResPath)) {
                var index = assetPath.IndexOf(PathUtility.ResPath, StringComparison.Ordinal);
                if (index != -1) {
                    assetPath = assetPath.Substring(index + PathUtility.ResPath.Length + 1);
                }
            }
            
            var extension = Path.GetExtension(assetPath);
            var assetBundleExtension = PathUtility.AssetBundleExtension;
            if(assetPath.Contains("Prefabs") && extension == ".prefab"){
                var pathWithoutExtension = Path.GetDirectoryName(assetPath) + "/" + Path.GetFileNameWithoutExtension(assetPath);
                return pathWithoutExtension + assetBundleExtension;
            }else{
                var pathWithoutFileName = Path.GetDirectoryName(assetPath);
                return pathWithoutFileName + assetBundleExtension;
            }
        }

    }
}