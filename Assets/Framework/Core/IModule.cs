

using System.Collections;

namespace Framework.Core
{
    /// <summary>
    /// 模块接口
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// 模块加载
        /// </summary>
        /// <param name="bundle"></param>
        void OnLoad(Bundle bundle);


        /// <summary>
        /// 模块卸载
        /// </summary>
        void OnUnload(Bundle bundle);

        /// <summary>
        /// 模块打开
        /// </summary>
        /// <param name="bundle"></param>
        void OnOpen(Bundle bundle);

        /// <summary>
        /// 模块关闭
        /// </summary>
        void OnClose(Bundle bundle);
    }
}
