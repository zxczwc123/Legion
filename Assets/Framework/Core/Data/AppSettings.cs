using UnityEngine;

namespace Framework.Core
{
    [CreateAssetMenu(menuName = "Create GameSettings", fileName = "GameSettings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        /// <summary>
        /// 启动模块
        /// </summary>
        [SerializeField] public string startModule;

        /// <summary>
        /// 是否热更新
        /// </summary>
        [SerializeField] public bool isHotFix;

        /// <summary>
        /// 是否加载Dll
        /// </summary>
        [SerializeField] public bool isDLL;

        /// <summary>
        /// 是否Debug
        /// </summary>
        [SerializeField] public bool isDebug;

        /// <summary>
        /// 是否assetBundle资源
        /// </summary>
        [SerializeField] public bool isAssetBundle;
    }
}