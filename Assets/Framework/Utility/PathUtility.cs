using Framework.Core;
using System.IO;
using UnityEngine;

namespace Framework.Utility
{
    public class PathUtility
    {
        public static string AssetBundleExtension = ".ab";

        /// <summary>
        /// 资源存放路径
        /// </summary>
        /// <value></value>
        public static string ResPath
        {
            get { return "Res"; }
        }

        /// <summary>
        /// 优先从persistent获取 没有则从streaming获取
        /// </summary>
        public static string GetAssetsPath(string fileName)
        {
            var filePath = Path.Combine(GetPersistentPath(), fileName);
            if (File.Exists(filePath))
            {
                return filePath;
            }
            filePath = Path.Combine(GetStreamingPath(), fileName);
            return filePath;
        }

        /// <summary>
        /// 优先从persistent获取 没有则从streaming获取
        /// </summary>
        public static string GetAssetsPlatformPath(string fileName)
        {
            if (FrameworkEngine.Instance.isHotFix)
            {
                var filePath = Path.Combine(GetPersistentAssetPlatformPath(), fileName);
                if (File.Exists(filePath))
                {
                    return filePath;
                }
                filePath = Path.Combine(GetStreamingAssetPlatformPath(), fileName);
                return filePath;
            }
            else
            {
                var filePath = Path.Combine(GetStreamingAssetPlatformPath(), fileName);
                return filePath;
            }
        }

        /// <summary>
        /// 配置路径
        /// </summary>
        public static string GetStreamingConfigPath()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Config");
            return path;
        }

        public static string GetStreamingVersionPath()
        {
            string versionPath = Path.Combine(GetStreamingAssetPlatformPath(), GetVersionFileName());
            return versionPath;
        }

        public static string GetPersistentVersionPath()
        {
            string versionPath = Path.Combine(GetPersistentAssetPlatformPath(), GetVersionFileName());
            return versionPath;
        }

        public static string GetStreamingHotFixDllPath()
        {
            string versionPath = Path.Combine(GetStreamingAssetPlatformPath(), GetHotFixDllFileName());
            return versionPath;
        }

        public static string GetPersistentHotFixDllPath()
        {
            string versionPath = Path.Combine(GetPersistentAssetPlatformPath(), GetHotFixDllFileName());
            return versionPath;
        }

        public static string GetStreamingAssetPlatformPath()
        {
            string path = Path.Combine(Application.streamingAssetsPath, GetAssetPlatformRoot());
            return path;
        }

        public static string GetPersistentAssetPlatformPath()
        {
            string path = Path.Combine(Application.persistentDataPath, GetAssetPlatformRoot());
            return path;
        }

        public static string GetPersistentPath()
        {
            return Application.persistentDataPath;
        }

        public static string GetStreamingPath()
        {
            return Application.streamingAssetsPath;
        }

        /// <summary>
        /// 热更信息文件清单
        /// </summary>
        /// <returns></returns>
        public static string GetVersionFileName()
        {
            return "version.manifest";
        }

        /// <summary>
        /// 热更信息文件清单
        /// </summary>
        /// <returns></returns>
        public static string GetHotFixDllFileName()
        {
            return "game.dll";
        }

        /// <summary>
        /// 平台目录
        /// </summary>
        /// <returns></returns>
        public static string GetAssetPlatformRoot()
        {
#if UNITY_ANDROID
            string path = "AssetBundle_And";
#elif UNITY_IOS
            string path = "AssetBundle_Ios";
#else
            string path = "AssetBundle";
#endif
            return path;
        }

#if UNITY_EDITOR
        public static string GetAndroidAssetRoot()
        {
            string path = "AssetBundle_And";
            return path;
        }

        public static string GetIosAssetRoot()
        {
            string path = "AssetBundle_Ios";
            return path;
        }

        public static string GetStreamingAssetAndroidPath()
        {
            string path = Path.Combine(Application.streamingAssetsPath, GetAndroidAssetRoot());
            return path;
        }

        public static string GetStreamingAssetIosPath()
        {
            string path = Path.Combine(Application.streamingAssetsPath, GetIosAssetRoot());
            return path;
        }
#endif
    }
}