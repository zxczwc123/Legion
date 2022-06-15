// ========================================================
// 描 述：HotUpdateView.cs 
// 创 建： 
// 时 间：2020/09/15 16:17:23 
// 版 本：2018.2.20f1 
// ========================================================
using Game.Framework.UI;
using UnityEngine.UI;

namespace Game.Login {

    public class HotUpdateView : UIBaseView {

        public Text info;

        public Slider byteProgress;

        public Slider fileProgress;

        public Text labelFile;

        public Text labelByte;

        public Button btnCheck;

        public Button btnRetry;

        public Button btnUpdate;

        public Button btnDownload;

        public Button btnRestart;

        private IHotUpdatePresenter m_Presenter;
        
        protected void OnBind() {

            btnCheck = transform.Find("BtnGroup/BtnCheck").GetComponent<Button>();
            btnCheck.onClick.AddListener(() => {
                if (m_Presenter != null) m_Presenter.CheckUpdate();
            });

            btnRetry = transform.Find("BtnGroup/BtnRetry").GetComponent<Button>();
            btnRetry.onClick.AddListener(() => {
                if (m_Presenter != null) m_Presenter.Retry();
            });

            btnUpdate = transform.Find("BtnGroup/BtnUpdate").GetComponent<Button>();
            btnUpdate.onClick.AddListener(() => {
                if (m_Presenter != null) m_Presenter.HotUpdate();
            });

            btnDownload = transform.Find("BtnGroup/BtnDownload").GetComponent<Button>();
            btnDownload.onClick.AddListener(() => {
                if (m_Presenter != null) m_Presenter.Download();
            });

            btnRestart = transform.Find("BtnGroup/BtnRestart").GetComponent<Button>();
            btnRestart.onClick.AddListener(() => {
                if (m_Presenter != null) m_Presenter.Restart();
            });

            info = transform.Find("TextInfo").GetComponent<Text>();

            labelByte = transform.Find("ByteProgress/Text").GetComponent<Text>();

            labelFile = transform.Find("FileProgress/Text").GetComponent<Text>();

            byteProgress = transform.Find("ByteProgress").GetComponent<Slider>();

            fileProgress = transform.Find("FileProgress").GetComponent<Slider>();

            btnCheck.gameObject.SetActive(false);
            btnDownload.gameObject.SetActive(false);
            btnRetry.gameObject.SetActive(false);
            btnUpdate.gameObject.SetActive(false);
            btnRestart.gameObject.SetActive(false);
        }

        public void SetPresenter(IHotUpdatePresenter presenter) {
            m_Presenter = presenter;
        }
    }
}
