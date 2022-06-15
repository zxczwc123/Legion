namespace Framework.Native.WeChat {

    public class WeChatMgrWinClient : IWeChatMgrClient {

        public WeChatMgrWinClient(WeChatMgr weChatMgr) {

        }

        /// <summary>
        /// 加载初始化
        /// </summary>
        public void Load() {

        }

        /// <summary>
        /// 卸载
        /// </summary>
        public void Unload() {

        }

        /// <summary>
        /// 原生监听回调
        /// </summary>
        /// <param name="code"></param>
        public void OnLoginCallback(string code) {

        }

        /// <summary>
        /// 原生监听回调
        /// </summary>
        /// <param name="code"></param>
        public void OnShareCallback(int code) {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        public void OnPayCallback(int code) {

        }

        /// <summary>
        /// 微信是否安装
        /// </summary>
        /// <returns></returns>
        public bool IsWXAppInstalled() {
            return false;
        }

        /// <summary>
        /// 发起微信授权
        /// </summary>
        public void Login() {

        }

        /// <summary>
        /// 分享图片
        /// </summary>
        /// <param name="title"></param>
        /// <param name="imagePath"></param>
        /// <param name="description"></param>
        /// <param name="scene"></param>
        public void ShareImage(string title, string imagePath, string description, int scene) {

        }

        /// <summary>
        /// 分享链接
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="description"></param>
        /// <param name="scene"></param>
        public void ShareUrl(string title, string url, string description, int scene) {

        }

        /// <summary>
        /// 分享文本
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="scene"></param>
        public void ShareText(string title, string text, int scene) {

        }

        /// <summary>
        /// 支付
        /// </summary>
        public void Pay(string appid,string partnerid, string prepayid, string noncestr, string timestamp, string pack, string sign) {
            
        }

        public void LaunchMiniProgram(string username, string path) {
            
        }

    }
}
