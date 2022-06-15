namespace Game.Login
{
    public interface IHotUpdateListener
    {
        /// <summary>
        /// 已经是最新的版本 则显示登陆  关闭 更新界面
        /// </summary>
        void OnVersionUpToDate();

        /// <summary>
        /// 更新完成
        /// </summary>
        void OnVersionUpdateFinish();
    }
}