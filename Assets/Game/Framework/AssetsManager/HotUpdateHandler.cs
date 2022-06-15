// ========================================================
// 描 述：HotUpdateHandler.cs 
// 创 建： 
// 时 间：2020/09/15 16:17:24 
// 版 本：2018.2.20f1 
// ========================================================

using System;
using Game.Framework;
using UnityEngine;

namespace Game.Login
{
    public class HotUpdateHandler : IHotUpdatePresenter, IAssetsManagerListener
    {
        private HotUpdateView m_UpdateView;
        private bool m_Updating;
        private bool m_CanRetry;
        private AssetsManager m_AssetManager;
        private Action m_UpdateListener;
        public int failCount;

        private IHotUpdateListener m_HotUpdateListener;

        public void SetHotUpdateListener(IHotUpdateListener listener)
        {
            m_HotUpdateListener = listener;
        }

        public void SetHotUpdateView(HotUpdateView view)
        {
            m_UpdateView = view;
        }

        public string GetLocalVersion()
        {
            var localManifest = m_AssetManager.GetLocalManifest();
            if (localManifest == null)
            {
                return "";
            }
            var localVersion = localManifest.version;
            return localVersion;
        }

        public void CheckCallback(AssetEventData eventData)
        {
            Debug.Log("Code: " + eventData.eventType);
            switch (eventData.eventType)
            {
                case AssetEvent.ErrorNoLocalManifest:
                    m_UpdateView.info.text = "没有本地更新文件";
                    break;
                case AssetEvent.ErrorDownloadManifest:
                case AssetEvent.ErrorParseManifest:
                    m_UpdateView.info.text = "下载更新文件失败";
                    m_UpdateView.btnCheck.gameObject.SetActive(true);
                    break;
                case AssetEvent.AlreadyUpToDate:
                    m_UpdateView.info.text = "已经是最新版本";
                    if (m_HotUpdateListener != null) m_HotUpdateListener.OnVersionUpToDate();
                    break;
                default:
                    return;
            }
            m_AssetManager.SetListener(null);
            m_Updating = false;
        }

        public void UpdateCallback(AssetEventData eventData)
        {
            var needRestart = false;
            var failed = false;
            switch (eventData.eventType)
            {
                case AssetEvent.ErrorNoLocalManifest:
                    m_UpdateView.info.text = "没有本地更新文件, 跳过更新";
                    failed = true;
                    break;
                case AssetEvent.StorageOlder:
                    break;
                case AssetEvent.ErrorDownloadManifest:
                case AssetEvent.ErrorParseManifest:
                    m_UpdateView.info.text = "下载更新文件失败.";
                    m_UpdateView.btnCheck.gameObject.SetActive(true);
                    failed = true;
                    break;
                case AssetEvent.AlreadyUpToDate:
                    m_UpdateView.info.text = "已经是最新版本";
                    failed = true;
                    if (m_HotUpdateListener != null) m_HotUpdateListener.OnVersionUpToDate();
                    break;
                case AssetEvent.NewVersionFound:
                    Debug.Log(eventData.eventType);
                    // 小版本更新
                    var remoteManifest = m_AssetManager.GetRemoteManifest();
                    m_UpdateView.info.text = string.Format("发现新版本{0}，请更新", remoteManifest.version);
                    m_UpdateView.btnCheck.gameObject.SetActive(false);
                    m_UpdateView.btnUpdate.gameObject.SetActive(true);
                    m_UpdateView.fileProgress.value = 0;
                    m_UpdateView.byteProgress.value = 0;
                    break;
                case AssetEvent.BiggerVersionFound:
                    // 大版本更新
                    var remoteM = m_AssetManager.GetRemoteManifest();
                    m_UpdateView.info.text = string.Format("发现新版本{0}，请下载新版本", remoteM.version);
                    m_UpdateView.btnCheck.gameObject.SetActive(false);
                    m_UpdateView.btnDownload.gameObject.SetActive(true);
                    break;
                case AssetEvent.UpdateProgression:
                    m_UpdateView.fileProgress.gameObject.SetActive(true);
                    m_UpdateView.fileProgress.value = eventData.progress;
                    m_UpdateView.labelFile.text = eventData.currentFileCount + " / " + eventData.totalFileCount;
                    m_UpdateView.info.text = "正在下载更新文件";
                    break;
                case AssetEvent.UpdateFinished:
                    m_UpdateView.info.text = "更新完成,请重新启动。 ";
                    needRestart = true;
                    break;
                case AssetEvent.UpdateFailed:
                    m_UpdateView.info.text = string.Format("更新失败, {0}", eventData.message);
                    m_UpdateView.btnRetry.gameObject.SetActive(true);
                    m_Updating = false;
                    m_CanRetry = true;
                    break;
                case AssetEvent.ErrorUpdating:
                    m_UpdateView.info.text = "资源更新错误: ";
                    break;
                case AssetEvent.ErrorDecompress:
                    m_UpdateView.info.text = "";
                    break;
            }

            if (failed)
            {
                m_AssetManager.SetListener(null);
                m_UpdateListener = null;
                m_Updating = false;
            }

            if (needRestart)
            {
                m_AssetManager.SetListener(null);
                m_UpdateListener = null;

                //m_UpdateView.btnRestart.gameObject.SetActive(true);
                m_HotUpdateListener.OnVersionUpdateFinish();
            }
        }

        public void Retry()
        {
            if (!m_Updating && m_CanRetry)
            {
                m_UpdateView.btnRetry.gameObject.SetActive(false);
                m_CanRetry = false;

                m_UpdateView.info.text = "重新下载更新失败资源";
                m_Updating = true;
                m_AssetManager.HotUpdate();
            }
        }

        public void Download()
        {
            Application.OpenURL(GameConfig.DownloadUrl);
        }

        public void Restart()
        {
            Application.Quit();
        }

        public void CheckUpdate()
        {
            if (m_Updating)
            {
                m_UpdateView.info.text = "正在检查更新文件...";
                return;
            }
            m_UpdateView.info.text = "开始检查更新文件";
            m_UpdateView.fileProgress.gameObject.SetActive(false);
            m_AssetManager.SetListener(this);

            m_AssetManager.CheckHotUpdate();
        }

        public void HotUpdate()
        {
            if (m_AssetManager != null && !m_Updating)
            {
                Debug.Log("start update");
                m_AssetManager.SetListener(this);
                failCount = 0;
                m_UpdateView.btnUpdate.gameObject.SetActive(false);
                m_Updating = true;
                m_AssetManager.HotUpdate();
            }
            else
            {
                Debug.LogWarning("asset null or isupdating");
            }
        }

        public void Show()
        {
            m_UpdateView.Show();
        }

        /// <summary>
        /// use this for initialization
        /// </summary>
        public void OnLoad()
        {
            m_AssetManager = new AssetsManager();
            m_UpdateView.fileProgress.value = 0;
            m_UpdateView.byteProgress.value = 0;
            m_UpdateView.info.text = "更新准备就绪";
            CheckUpdate();
        }

        public void OnUnload()
        {
            if (m_UpdateListener != null)
            {
                m_AssetManager.SetListener(null);
                m_UpdateListener = null;
            }
        }

        public void OnUpdateCallback(AssetEventData eventData)
        {
            UpdateCallback(eventData);
        }
    }
}