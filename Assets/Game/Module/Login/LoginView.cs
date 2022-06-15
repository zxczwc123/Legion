// ========================================================
// 描 述：LoginView.cs 
// 作 者： 
// 时 间：2020/07/23 20:50:44 
// 版 本：2019.2.1f1 
// ========================================================
using System;
using Game.Framework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Login {
    public class LoginView : UIView {

        public Action OnLogin;

        public Action OnService;

        private Button m_BtnLogin;

        private InputField m_inputId;

        private GameObject m_objLogin;

        private Text m_textVersion;

        private Toggle m_togPrivacy;

        public CanvasGroup canvasGroup;

        public LoginView(RectTransform transform) : base(transform) {

            canvasGroup = transform.GetComponent<CanvasGroup>();

            m_textVersion = transform.Find("TextVersion").GetComponent<Text>();

            m_objLogin = transform.Find("LoginRoot").gameObject;

            m_inputId = transform.Find("LoginRoot/InputField").GetComponent<InputField>();

            m_togPrivacy = transform.Find("LoginRoot/TogglePrivacy").GetComponent<Toggle>();

            m_BtnLogin = transform.Find("LoginRoot/BtnLogin").GetComponent<Button>();
            m_BtnLogin.onClick.AddListener(() => {
                if (OnLogin != null) OnLogin();
            });

            var btnService = transform.Find("BtnService").GetComponent<Button>();
            btnService.onClick.AddListener(() => {
                if (OnService != null) OnService();
            });
        }

        public void SetVersion(string version) {
            this.m_textVersion.text = string.Format("version: {0}", version);
        }

        public void SetLoginActive(bool active) {
            this.m_objLogin.SetActive(active);
        }

        public void SetBtnLoginActive(bool active) {
            this.m_BtnLogin.gameObject.SetActive(active);
        }

        public void SetPlayerId(bool active, string playerId) {
            this.m_inputId.gameObject.SetActive(active);
            this.m_inputId.text = playerId;
        }

        public string GetPlayerId() {
            if (!this.m_inputId.gameObject.activeSelf) {
                return null;
            }
            return this.m_inputId.text;
        }

        public bool GetPrivacyAgree() {
            return m_togPrivacy.isOn;
        }
    }
}