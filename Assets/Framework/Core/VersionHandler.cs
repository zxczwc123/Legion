// ========================================================
// 描 述：HotfixManager.cs 
// 作 者： 
// 时 间：2020/05/07 21:41:29 
// 版 本：2019.2.1f1 
// ========================================================
using System.Collections.Generic;
using System.IO;
using System.Text;
using Framework.Native.Util;
using Framework.Utility;
using UnityEngine;

namespace Framework.Core {

    public class VersionFileInfo {
        public string fileName;
        public string filePath;
    }

    public class VersionHandler {

        /// <summary>
        /// 本地的清单文件
        /// </summary>
        private Manifest m_persistentManifest;

        private Manifest m_streamingManifest;

        private Manifest m_localManifest;

        public VersionHandler() {
            // android assets 目录下不能用file.exists来验证文件存在与否
            // android assets 目录为非正常文件夹目录相当于包体 所以 也不能用File 来读取
            // streamingPath在 assets 目录下 所以 streamingPath下的资源最好都不用 File来读取
            var streamingVersionPath = PathUtility.GetStreamingVersionPath();
            var versionInfo = this.GetStreamingText(streamingVersionPath);
            if(versionInfo != null) {
                m_streamingManifest = LitJson.JsonMapper.ToObject<Manifest>(versionInfo);
            }
            if (m_streamingManifest == null) {
                Debug.LogError(string.Format("streamingManifest parse error ,filepath: {0}", streamingVersionPath));
                return;
            }
            var persistentVersionPath = PathUtility.GetPersistentVersionPath();
            if (File.Exists(persistentVersionPath)) {
                using (StreamReader sr = new StreamReader(File.OpenRead(persistentVersionPath), Encoding.UTF8)) {
                    var text = sr.ReadToEnd();
                    m_persistentManifest = LitJson.JsonMapper.ToObject<Manifest>(text);
                }
            }
        }

        public void HandleAssets() {
            if (m_persistentManifest != null) {
                Debug.Log(string.Format("streamingVersion: {0},persistentVersion: {1}", m_streamingManifest.version, m_persistentManifest.version));
                if (string.IsNullOrEmpty(m_persistentManifest.version)) {
                    // 存在异常情况
                    ClearPersisentAssets();
                    return;
                }
                var compareVersion = CompareVersion(m_streamingManifest.version, m_persistentManifest.version);
                if (compareVersion <= 0) {
                    m_localManifest = m_persistentManifest;
                    // 缓存的版本存在的时候一定要验证md5 可能由于各种未知情况导致md5对不上
                    // 如下载文件完成后从临时文件复制到缓存文件时产生问题 导致复制只复制了一半等等
                    var files = CheckFiles(m_localManifest);
                    if(files.Count > 0) {
                        // 存在异常情况
                        ClearPersisentAssets();
                    }
                } else {
                    // 资源包的版本比缓存的版本高的情况，说明应该是新安装的情况 导致，所以清空缓存
                    m_localManifest = m_streamingManifest;
                    
                    ClearPersisentAssets();
                }
            } else {
                m_localManifest = m_streamingManifest;
            }
        }

        /// <summary>
        /// 比较版本大小
        /// A 大于 B 返回 1
        /// </summary>
        /// <param name="versionA"></param>
        /// <param name="versionB"></param>
        /// <returns></returns>
        public int CompareVersion(string versionA, string versionB) {
            var vA = versionA.Split('.');
            var vB = versionB.Split('.');
            for (var i = 0; i < vA.Length; ++i) {
                var a = int.Parse(vA[i]);
                var b = int.Parse(vB[i]);
                if (a == b) {
                    continue;
                } else {
                    return a - b;
                }
            }
            if (vB.Length > vA.Length) {
                return -1;
            } else {
                return 0;
            }
        }

        public void ClearPersisentAssets() {
            Directory.Delete(Application.persistentDataPath, true);
        }

        private string GetStreamingText(string filename) {
            string context = null;
            if (Application.platform == RuntimePlatform.Android) {
                WWW www = new WWW(filename);
                while (!www.isDone) { }
                context = www.text;
            } else {
                using (StreamReader sr = new StreamReader(File.OpenRead(filename), Encoding.UTF8)) {
                    context = sr.ReadToEnd();
                }
            }
            return context;
        }

        public static List<VersionFileInfo> CheckFiles(Manifest manifest) {
            var versionFileInfos = new List<VersionFileInfo>();
            foreach (var child in manifest.assets.Keys) {
                var asset = manifest.assets[child];
                var md5 = asset.md5;
                var fileName = child;
                var filePath = Path.Combine(PathUtility.GetPersistentAssetPlatformPath(), fileName);
                if (!System.IO.File.Exists(filePath)) {
                    if (Application.platform == RuntimePlatform.Android) {
                        var platformRootPath = PathUtility.GetAssetPlatformRoot();
                        var md5Old = CommonUtil.GetMD5FromAndroidStreaming(Path.Combine(platformRootPath, fileName));
                        if (md5 != md5Old) {
                            var willUpdateFile = new VersionFileInfo();
                            willUpdateFile.fileName = fileName;
                            willUpdateFile.filePath = filePath;
                            versionFileInfos.Add(willUpdateFile);
                            Debug.Log(string.Format("condition1: filename: {0} md5:{1}, md5old: {2}", fileName, md5, md5Old));
                        }
                    } else {
                        var streamingFilePath = Path.Combine(PathUtility.GetStreamingAssetPlatformPath(), fileName);
                        if (!File.Exists(streamingFilePath)) {
                            var willUpdateFile = new VersionFileInfo();
                            willUpdateFile.fileName = fileName;
                            willUpdateFile.filePath = filePath;
                            versionFileInfos.Add(willUpdateFile);
                        } else {
                            var md5Old = MD5Utility.GetMD5HashFromFile(streamingFilePath);
                            if (md5 != md5Old) {
                                var versionFileInfo = new VersionFileInfo();
                                versionFileInfo.fileName = fileName;
                                versionFileInfo.filePath = filePath;
                                versionFileInfos.Add(versionFileInfo);
                                Debug.Log(string.Format("condition2: filename: {0} md5:{1}, md5old: {2}", fileName, md5, md5Old));
                            }
                        }
                    }
                } else {
                    var md5Old = MD5Utility.GetMD5HashFromFile(filePath);
                    if (md5 != md5Old) {
                        var versionFileInfo = new VersionFileInfo();
                        versionFileInfo.fileName = fileName;
                        versionFileInfo.filePath = filePath;
                        versionFileInfos.Add(versionFileInfo);
                        Debug.Log(string.Format("condition3: filename: {0} md5:{1}, md5old: {2}", fileName, md5, md5Old));
                    }
                }
            }
            return versionFileInfos;
        }
    }
}