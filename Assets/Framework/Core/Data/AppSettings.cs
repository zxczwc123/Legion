using UnityEngine;
using UnityEngine.Serialization;

namespace Framework.Core
{
    [CreateAssetMenu(menuName = "Create AppSettings", fileName = "AppSettings", order = 0)]
    public class AppSettings : ScriptableObject
    {
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