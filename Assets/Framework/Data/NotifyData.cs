// ========================================================
// 描 述：NotifyData.cs 
// 作 者：郑贤春 
// 时 间：2019/06/26 08:00:22 
// 版 本：2018.3.12f1 
// ========================================================
// ========================================================
// 描 述：UserInfo.cs 
// 作 者：郑贤春 
// 时 间：2019/06/20 20:23:36 
// 版 本：2018.3.12f1 
// ========================================================
using System;

namespace Framework.Data {
    public struct NotifyData<T> {

        public Action<T> OnDataChange;

        private T m_value;
        public T Value {
            set {
                m_value = value;
                if (!m_stopNotify && OnDataChange != null) {
                    OnDataChange (m_value);
                }
            }
            get {
                return m_value;
            }
        }

        /// <summary>
        /// 是否通知 默认为true
        /// </summary>
        private bool m_stopNotify;

        public void DisableNotify(){
            m_stopNotify = true;
        }

        /// <summary>
        /// 启动通知
        /// </summary>
        public void EnableNotify(){
            m_stopNotify = false;
        }

        public void NotifyDataChange () {
            if (OnDataChange != null) {
                OnDataChange (m_value);
            }
        }
    }
}