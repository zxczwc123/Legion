namespace Game.Framework
{
    public interface IAssetsManagerListener {
        void OnUpdateCallback(AssetEventData eventData);
    }
}