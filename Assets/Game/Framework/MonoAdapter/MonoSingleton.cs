// ========================================================
// 描 述：BaseView.cs 
// 创 建： 
// 时 间：2020/09/29 11:31:07 
// 版 本：2018.2.20f1 
// ========================================================

using Framework.Core;
using UnityEngine;

namespace Game.Common {
    /// <summary>
    /// 管理器
    /// </summary>
    public abstract class MonoAdapterManager<T> : MonoAdapterObject where T : MonoAdapterManager<T>, new() {

        private static T s_Instance;

        /// <summary>
        /// 获取管理器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T instance {
            get {
                if (s_Instance == null) {
                    var managerGameObject = new GameObject(typeof(T).Name);
                    GameObject.DontDestroyOnLoad(managerGameObject);
                    s_Instance = new T();
                    s_Instance.Init(managerGameObject.transform);
                    s_Instance.OnCreate();
                }
                return s_Instance;
            }
        }

        protected void OnCreate() {
            m_Adapter.onDestroy = OnDestroyInternal;
        }

        private void OnDestroyInternal() {
            OnDestroy();
            s_Instance = null;
        }

        protected abstract void OnDestroy();
    }
}
