// ========================================================
// 描 述：AssetsManager.cs 
// 创 建： 
// 时 间：2020/09/11 15:58:46 
// 版 本：2018.2.20f1 
// ========================================================
using Framework.Core;
using Framework.Net;
using Framework.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Framework {
    /// <summary>
    /// 版本号说明  示列1.0.0
    /// 大版本下载更新 修改第一位数
    /// 新增功能修改 修改第二位数
    /// bug修复 修改第三位数
    /// </summary>
    public class AssetsManager {

        private string m_tempAssetsPath;


        private IAssetsManagerListener m_listener;

        /// <summary>
        /// 清单文件
        /// </summary>
        private Manifest m_streamingManifest;
        /// <summary>
        /// 清单文件
        /// </summary>
        private Manifest m_persistentManifest;
        /// <summary>
        /// 本地的清单文件 与 streaming 与 persistent 比较的更大值  相同取persistent
        /// </summary>
        private Manifest m_localManifest;
        /// <summary>
        /// 远程清单文件
        /// </summary>
        private Manifest m_remoteManifest;

        public Manifest GetStreamingManifest(){
            return m_localManifest;
        }

        public Manifest GetPersistentManifest() {
            return m_localManifest;
        }

        public Manifest GetLocalManifest() {
            return m_localManifest;
        }

        public Manifest GetRemoteManifest() {
            return m_remoteManifest;
        }

        /// <summary>
        /// 要更新的文件信息
        /// </summary>
        private List<VersionFileInfo> m_willUpdateFiles;

        public AssetsManager() {
            m_tempAssetsPath = Application.persistentDataPath + "/AssetBundle_Temp";
        }

        public void SetListener(IAssetsManagerListener listener) {
            m_listener = listener;
        }

        /// <summary>
        /// 检查更新情况
        /// </summary>
        public void CheckHotUpdate() {
            if (!Directory.Exists(m_tempAssetsPath)) {
                Directory.CreateDirectory(m_tempAssetsPath);
            }
            // android assets 目录下不能用file.exists来验证文件存在与否
            // android assets 目录为非正常文件夹目录相当于包体 所以 也不能用File 来读取
            // streamingPath在 assets 目录下 所以 streamingPath下的资源最好都不用 File来读取
            var streamingVersionPath = PathUtility.GetStreamingVersionPath();
            var versionInfo = GetStreamingText(streamingVersionPath);
            m_streamingManifest = LitJson.JsonMapper.ToObject<Manifest>(versionInfo);
            if (m_streamingManifest == null) {
                Debug.LogError(string.Format("streamingManifest parse error ,filepath: {0}", streamingVersionPath));
                return;
            }
            m_localManifest = m_streamingManifest;
            var persistentVersionPath = PathUtility.GetPersistentVersionPath();
            if (File.Exists(persistentVersionPath)) {
                using (StreamReader sr = new StreamReader(File.OpenRead(persistentVersionPath), Encoding.UTF8)) {
                    var text = sr.ReadToEnd();
                    m_persistentManifest = LitJson.JsonMapper.ToObject<Manifest>(text);
                }
            }
            if (m_persistentManifest != null) {
                Debug.Log(string.Format("streamingVersion: {0},persistentVersion: {1}", m_streamingManifest.version, m_persistentManifest.version));
                var compareVersion = CompareVersion(m_streamingManifest.version, m_persistentManifest.version);
                if (compareVersion <= 0) {
                    //  m_persistentManifest 的版本大于等于 m_streamingManifest
                    m_localManifest = m_persistentManifest;
                } else {
                    // m_streamingManifest 的版本大于  m_persistentManifest 说明打包的包体比较新 下载的资源比较老
                    ClearPersistentAssets();
                    var eventData = new AssetEventData();
                    eventData.eventType = AssetEvent.StorageOlder;
                    if (m_listener != null) m_listener.OnUpdateCallback(eventData);
                }
            } else {
                m_localManifest = m_streamingManifest;
            }
            var versionUrl = m_streamingManifest.packageUrl + "/" + PathUtility.GetAssetPlatformRoot() + "/" + PathUtility.GetVersionFileName();
            var versionTempPath = Path.Combine(m_tempAssetsPath, PathUtility.GetVersionFileName());
            HttpClient.Download(versionUrl, versionTempPath, (err,progress) => {
                
                if (err != null) {
                    var eventData = new AssetEventData();
                    eventData.eventType = AssetEvent.ErrorDownloadManifest;
                    if (m_listener != null) m_listener.OnUpdateCallback(eventData);
                } else if (progress == 1) {
                    OnVersionCheck();
                }
            });
        }

        /// <summary>
        /// 开始更新
        /// 此方法 需要提前CheckUpdate 如果没有 获取到远程清单则跟新失败 
        /// </summary>
        public void HotUpdate() {
            if (m_remoteManifest == null) {
                var eventData = new AssetEventData();
                eventData.eventType = AssetEvent.UpdateFailed;
                eventData.message = "更新文件清单下载失败";
                if (m_listener != null) m_listener.OnUpdateCallback(eventData);
                return;
            }
            if (m_willUpdateFiles.Count == 0) {
                // 没有需要的更新文件 更新完毕 把远程清单文件从临时移到 persistent目录
                CopyVersionFiles(m_tempAssetsPath, PathUtility.GetPersistentAssetPlatformPath());
                Directory.Delete(m_tempAssetsPath,true);
                var eventData = new AssetEventData();
                eventData.eventType = AssetEvent.UpdateFinished;
                if (m_listener != null) m_listener.OnUpdateCallback(eventData);
            } else {
                var failFileCount = 0;
                var downloadCount = 0;
                var allDownloadCount = m_willUpdateFiles.Count;
                var progressEventData = new AssetEventData();
                var progresses = new float[allDownloadCount];
                progressEventData.eventType = AssetEvent.UpdateProgression;
                progressEventData.totalFileCount = allDownloadCount;
                progressEventData.currentFileCount = downloadCount;
                for (var i=0;i<allDownloadCount;i++) {
                    var index = i;
                    var willUpdateFile = m_willUpdateFiles[i];
                    var fileName = willUpdateFile.fileName;

                    var downloadFileUrl = m_localManifest.packageUrl + "/" + PathUtility.GetAssetPlatformRoot() + "/" + fileName;
                    var downloadFilePath = m_tempAssetsPath + "/" + willUpdateFile.fileName;
                    Debug.Log(string.Format("Download file : {0} ,path :{1}", fileName, downloadFilePath));
                    HttpClient.Download(downloadFileUrl, downloadFilePath, (err, progress) => {
                        if (err != null) {
                            progresses[index] = 1f;
                            downloadCount++;
                            failFileCount++;
                            progressEventData.currentFileCount = downloadCount;
                            progressEventData.progress = GetAllFileProgress(progresses);
                            if (m_listener != null) m_listener.OnUpdateCallback(progressEventData);
                        } else if (progress == 1) {
                            progresses[index] = 1f;
                            downloadCount++;
                            progressEventData.currentFileCount = downloadCount;
                            progressEventData.progress = GetAllFileProgress(progresses);
                            if (m_listener != null) m_listener.OnUpdateCallback(progressEventData);
                        } else {
                            progresses[index] = progress;
                            progressEventData.currentFileCount = downloadCount;
                            progressEventData.progress = GetAllFileProgress(progresses);
                            if (m_listener != null) m_listener.OnUpdateCallback(progressEventData);
                        }
                        if (downloadCount == allDownloadCount) {
                            if(failFileCount != 0) {
                                var eventData = new AssetEventData();
                                eventData.eventType = AssetEvent.UpdateFailed;
                                eventData.message = "下载文件失败";
                                if (m_listener != null) m_listener.OnUpdateCallback(eventData);
                            } else {
                                // 更新完毕 把文件从临时目录复制到缓存目录
                                // 下载完毕 再次验证资源 防止下载的资源对不上
                                CopyVersionFiles(m_tempAssetsPath, PathUtility.GetPersistentAssetPlatformPath());
                                Directory.Delete(m_tempAssetsPath,true);
                                if (VersionHandler.CheckFiles(m_remoteManifest).Count == 0) {
                                    var eventData = new AssetEventData();
                                    eventData.eventType = AssetEvent.UpdateFinished;
                                    if (m_listener != null) m_listener.OnUpdateCallback(eventData);
                                } else {
                                    var eventData = new AssetEventData();
                                    eventData.eventType = AssetEvent.UpdateFailed;
                                    eventData.message = "更新文件错误";
                                    if (m_listener != null) m_listener.OnUpdateCallback(eventData);
                                }
                            }
                        } 
                    });
                }
            }
        }

        private float GetAllFileProgress(float[] progresses) {
            var progress = 0f;
            for(var i = 0; i < progresses.Length; i++) {
                progress += progresses[i];
            }
            return progress / progresses.Length;
        }

        public void CopyVersionFiles(string sourceFolder, string destFolder) {
            try {
                //如果目标路径不存在,则创建目标路径
                if (!Directory.Exists(destFolder)) {
                    Directory.CreateDirectory(destFolder);
                }
                //得到原文件根目录下的所有文件
                string[] files = Directory.GetFiles(sourceFolder);
                foreach (string file in files) {
                    string name = Path.GetFileName(file);
                    string dest = Path.Combine(destFolder, name);
                    File.Copy(file, dest, true);//复制文件
                }
                //得到原文件根目录下的所有文件夹
                string[] folders = Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders) {
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name);
                    CopyVersionFiles(folder, dest);//构建目标路径,递归复制文件
                }

            } catch (Exception e) {
                Debug.LogError(e.ToString());
            }
        }

        /// <summary>
        /// 比较版本大小
        /// </summary>
        /// <param name="versionA"></param>
        /// <param name="versionB"></param>
        /// <returns></returns>
        private int CompareVersion(string versionA, string versionB) {
            var vA = versionA.Split('.');
            var vB = versionB.Split('.');
            for (var i = 0; i < vA.Length; ++i) {
                var a = int.Parse(vA[i]);
                var b = int.Parse(vB[i]);
                if (a != b) {
                    return a - b;
                } 
            }
            if (vB.Length > vA.Length) {
                return -1;
            } else {
                return 0;
            }
        }

        /// <summary>
        /// 比较与远程版本
        /// </summary>
        private void OnVersionCheck() {
            var versionTempPath = Path.Combine(m_tempAssetsPath, PathUtility.GetVersionFileName());
            using (StreamReader sr = new StreamReader(File.OpenRead(versionTempPath), Encoding.UTF8)) {
                var versionInfo = sr.ReadToEnd();
                m_remoteManifest = LitJson.JsonMapper.ToObject<Manifest>(versionInfo);
            }
            if (m_remoteManifest == null) {
                var eventData = new AssetEventData();
                eventData.eventType = AssetEvent.ErrorParseManifest;
                if (m_listener != null) m_listener.OnUpdateCallback(eventData);
                return;
            }
            Debug.Log(string.Format("localVersion: {0},remoteVersion: {1}", m_localManifest.version, m_remoteManifest.version));
            if (int.Parse(m_localManifest.version.Split('.')[0]) < int.Parse(m_remoteManifest.version.Split('.')[0])) {
                // 大版本更新
                var eventData = new AssetEventData();
                eventData.eventType = AssetEvent.BiggerVersionFound;
                if (m_listener != null) m_listener.OnUpdateCallback(eventData);
                return;
            }
            // 小版本
            var compareVersion = CompareVersion(m_localManifest.version, m_remoteManifest.version);
            if (compareVersion > 0) {
                // 本地版本更高的情况
                var eventData = new AssetEventData();
                eventData.eventType = AssetEvent.AlreadyUpToDate;
                if (m_listener != null) m_listener.OnUpdateCallback(eventData);
            } else {
                // 远程版本 大于 或 等于本地版本的情况 
                // 此处的md5对比 可修改为 m_remoteManifest 与 m_localManifest的对比 因为 已经对比过一次m_localManifest 与 本地资源的md5
                m_willUpdateFiles = VersionHandler.CheckFiles(m_remoteManifest);
                if (compareVersion == 0 && m_willUpdateFiles.Count == 0) {
                    // 版本相同的情况 又没有文件变化 不需要更新
                    var eventData = new AssetEventData();
                    eventData.eventType = AssetEvent.AlreadyUpToDate;
                    if (m_listener != null) m_listener.OnUpdateCallback(eventData);
                } else {
                    // 版本不同 或者 相同有文件变化
                    var eventData = new AssetEventData();
                    eventData.eventType = AssetEvent.NewVersionFound;
                    if (m_listener != null) m_listener.OnUpdateCallback(eventData);
                }
            }
        }

        private void ClearPersistentAssets() {
            Directory.Delete(Application.persistentDataPath,true);
        }

        private string GetStreamingText(string filename) {
            string context;
            if (Application.platform == RuntimePlatform.Android) {
                var request = new UnityWebRequest(filename);
                while (!request.isDone) { }
                context = request.downloadHandler.text;
            } else {
                using (StreamReader sr = new StreamReader(File.OpenRead(filename), Encoding.UTF8)) {
                    context = sr.ReadToEnd();
                }
            }
            return context;
        }
    }
}
