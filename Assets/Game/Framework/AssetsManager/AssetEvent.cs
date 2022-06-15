namespace Game.Framework
{
    public enum AssetEvent
    {
        ErrorNoLocalManifest = 0,
        ErrorDownloadManifest = 1,
        ErrorParseManifest = 2,
        AlreadyUpToDate = 3,
        
        NewVersionFound = 4, // 小版本， 只需要热更
        BiggerVersionFound = 11, // 大版本 发现需要整包下载
        UpdateFinished = 5,
        UpdateFailed = 6,
        UpdateProgression = 7,
        ErrorUpdating = 8,
        ErrorDecompress = 9,
        /// <summary>
        /// 下载的资源版本比安装包低，此时需要清空下载资源
        /// </summary>
        StorageOlder = 10,
    }
}