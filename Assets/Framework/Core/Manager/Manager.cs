// ========================================================
// 描 述：Manager.cs 
// 作 者：郑贤春 
// 时 间：2017/05/01 14:53:26 
// 版 本：5.5.2f1 
// ========================================================
using UnityEngine;

namespace Framework.Core {
    /// <summary>
    /// 管理器
    /// </summary>
    public abstract class Manager<T> : MonoBehaviour where T : Manager<T> {

        private static T instance;

        /// <summary>
        /// 获取管理器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Instance {
            get {
                if (instance == null) {
                    instance = Engine.instance.gameObject.AddComponent<T>();
                    instance.Init();
                }
                return instance;
            }
        }


        public virtual void Dispose()
        {
            
        }

        /// <summary>
        /// 管理器唤醒
        /// </summary>
        protected virtual void Init() {

        }
    }
}
