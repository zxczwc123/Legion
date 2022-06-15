namespace Game.Login
{
    public interface IHotUpdatePresenter
    {
        void Retry();

        void CheckUpdate();

        void HotUpdate();

        void Download();

        void Restart();
    }
}