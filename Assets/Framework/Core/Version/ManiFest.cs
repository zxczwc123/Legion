using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public class Manifest
    {
        /// <summary>
        /// 下载更新地址
        /// </summary>
        public string packageUrl;

        /// <summary>
        /// 版本号信息
        /// </summary>
        public string version;

        /// <summary>
        /// 所有资源信息 资源目录有对应平台目录 AssetBundle_Ios AssetBundle_And AssetBundle
        /// </summary>
        public Dictionary<string, AssetInfo> assets = new Dictionary<string, AssetInfo>();
    }

    public class AssetInfo
    {
        public int size;

        public string md5;
    }
}
