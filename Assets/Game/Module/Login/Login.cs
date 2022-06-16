// ========================================================
// 描 述：Login.cs 
// 作 者：郑贤春 
// 时 间：2017/05/01 16:05:46 
// 版 本：5.5.2f1 
// ========================================================
using System;
using System.Collections;
using DG.Tweening;
using Framework.Core;
using Framework.Core.MonoBehaviourAdapter;
using Framework.Native.AliPay;
using Framework.Native.WeChat;
using Framework.Splash;
using Game.Framework;
using Game.Framework.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Login {

    public class Login : Module, IHotUpdateListener {

        private LoginView m_View;

        private HotUpdateHandler m_HotUpdate;

        private HotUpdateView m_HotUpdateView;

        public override void OnLoad(Bundle bundle) {

            Debug.Log("Login onload");

            // 保持长亮
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            Application.targetFrameRate = 60;

            var viewEntity = UIManager.instance.LoadViewEntity("LoginView");
            m_View = new LoginView(viewEntity);

            m_View.OnLogin = OnLogin;
            m_View.OnService = OnServiceClick;

            var logsViewer = GameObject.Find("Reporter");
            if(logsViewer != null) {
                logsViewer.SetActive(Engine.appSettings.isDebug);
            }

            if (Engine.appSettings.isHotFix) {

                this.m_HotUpdate = new HotUpdateHandler();
                this.m_HotUpdate.SetHotUpdateListener(this);
                var hotUpdateTransform = this.m_View.transform.Find("HotUpdateView") as RectTransform;
                this.m_HotUpdateView = new HotUpdateView();
                this.m_HotUpdateView.BindWidget(hotUpdateTransform);
                this.m_HotUpdateView.SetPresenter(this.m_HotUpdate);
                this.m_HotUpdate.SetHotUpdateView(this.m_HotUpdateView);
            }

            UpdateManager.instance.AddUpdate(OnUpdate);
        }


        public override void OnUnload(Bundle bundle) {
            UIManager.instance.UnloadViewEntity("LoginView");
            UpdateManager.instance.DelUpdate(OnUpdate);
        }

        public override void OnOpen(Bundle bundle) {

            m_View.ShowAsNoMask();

            StartCoroutine(InintSingleMap());

            if (Engine.appSettings.isHotFix) {
                Debug.Log("启动更新");
                this.m_View.SetLoginActive(false);
                this.m_HotUpdateView.Show();
                this.m_HotUpdate.OnLoad();
                this.m_View.SetVersion(this.m_HotUpdate.GetLocalVersion());
            } else {
                this.m_View.SetLoginActive(true);
            }

            this.StartCoroutine(HideSplash());

            AliPayMgr.Instance.Load();
            WeChatMgr.Instance.Load();

        }

        private IEnumerator HideSplash() {
            yield return new WaitForSeconds(0.5f);
            SplashHandler.HideSplash();
        }

        public override void OnClose(Bundle bundle) {
            m_View.Hide();
        }

        private IEnumerator InintSingleMap()
        {
            UIManager.instance.LoadViewEntity("IslandSingleGameView");
            yield return new WaitForSeconds(0.5f);
            UIManager.instance.UnloadViewEntity("IslandSingleGameView");

        }

        private void OnServiceClick() {

        }

        private void OnUpdate() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                // 弹窗提示是否购买金币
                var dialogIntent = new Bundle();
                dialogIntent.stringValue = "确定要退出游戏吗？";
                dialogIntent.stringDict["ok"] = "确定";
                dialogIntent.callbackDict["ok"] = () => {
                    Application.Quit();
                };
                dialogIntent.stringDict["cancel"] = "取消";
                EventManager.Post("DialogQuit",dialogIntent);
            }
        }

        /// <summary>
        /// 登陆成功
        /// </summary>
        private void OnLoginSuccess() {
            var bundle = new Bundle();
            bundle.stringValue = "login";
            OpenModule("Hall", bundle);

            this.m_View.canvasGroup.DOFade(0f, 1f).SetEase(Ease.Linear).OnComplete(() => {
                this.m_View.canvasGroup.alpha = 1f;
                CloseModule("Login");
            });
        }

        /// <summary>
        /// 登录
        /// </summary>
        public void OnLogin() {
            var isAgree = this.m_View.GetPrivacyAgree();
            if (!isAgree) {
                var bundle = new Bundle();
                bundle.stringValue = "请先同意使用许可及服务协议！";
                EventManager.Post("Toast", bundle);
            }
        }

        public void OnLoginWeChatApp() {
            WeChatMgr.Instance.Login();
        }

        /// <summary>
        /// 启动一个脚本 切换场景来重新启动脚本引擎
        /// </summary>
        private void OnDownloadEnd() {
            var updateAdapterObject = new GameObject("SceneLoaderAdapter");
            var sceneLoaderAdapter = updateAdapterObject.AddComponent<MonoBehaviourAdapter>();
            sceneLoaderAdapter.StartCoroutine(StarMainSceneIE(updateAdapterObject));
            GameObject.DontDestroyOnLoad(updateAdapterObject);
        }

        IEnumerator StarMainSceneIE(GameObject sceneLoaderAdapterObject) {
            // 等待一帧 可以如按钮点击什么之类的完成，然engine 执行完
            yield return null;
            var obj = GameObject.Find("FrameworkEngine");
            GameObject.DestroyImmediate(obj);
            var op1 = SceneManager.LoadSceneAsync("Loading");
            yield return op1;
            var op2 = SceneManager.LoadSceneAsync("Login");
            yield return op2;
            GameObject.DestroyImmediate(sceneLoaderAdapterObject);
        }

        public void OnVersionUpToDate() {
            this.m_View.SetLoginActive(true);
            this.m_HotUpdateView.Hide();
        }

        public void OnVersionUpdateFinish() {
            this.OnDownloadEnd();
        }
    }
}