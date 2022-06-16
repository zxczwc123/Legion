using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Core;
using Framework.Core.MonoBehaviourAdapter;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Framework {
    public abstract class Module : IModule {
        /// <summary>
        /// 模块是否已经加载完毕
        /// 模块加载有一定的时间
        /// </summary>
        internal bool isLoaded;
        /// <summary>
        /// 模块是否已经打开
        /// 模块加载有一定的时间
        /// </summary>
        internal bool isOpen;
        /// <summary>
        /// 模块是否永久的
        /// 即加载后即使是切换场景仍是不可卸载的
        /// 非永久的，则在切换场景的时候被卸载
        /// </summary>
        internal bool isPermanent;
        /// <summary>
        /// 模块配置名称
        /// </summary>
        public string moduleName;
        /// <summary>
        /// 模块场景
        /// 游戏模块有场景限制
        /// 当场景切换时 非永久的模块将被卸载
        /// </summary>
        public string moduleScene;
        /// <summary>
        /// 
        /// </summary>
        public string[] dependViews;
        /// <summary>
        /// 
        /// </summary>
        public string[] dependModules;

        /// <summary>
        /// 事件系统
        /// </summary>
        private EventSystem m_eventSystem;

        /// <summary>
        /// 加载模块
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="bundle"></param>
        protected static void LoadModule (string moduleName, Bundle bundle = null) {
            ModuleManager.instance.LoadModule (moduleName, bundle);
        }

        /// <summary>
        /// 加载模块
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="bundle"></param>
        protected static void LoadModuleAysnc(string moduleName, Bundle bundle = null) {
            ModuleManager.instance.LoadModuleAsync(moduleName, bundle);
        }

        /// <summary>
        /// 卸载模块
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="bundle"></param>
        protected static void UnLoadModule (string moduleName, Bundle bundle = null) {

            ModuleManager.instance.UnloadModule (moduleName, bundle);
        }

        /// <summary>
        /// 打开模块
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="bundle"></param>
        protected static void OpenModule (string moduleName, Bundle bundle = null) {
            ModuleManager.instance.OpenModule (moduleName, bundle);
        }

        /// <summary>
        /// 打开模块 异步只限于加载过程，如果已经加载 则同步打开
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="bundle"></param>
        protected static void OpenModuleAsync(string moduleName, Bundle bundle = null) {
            ModuleManager.instance.OpenModuleAsync(moduleName, bundle);
        }

        /// <summary>
        /// 关闭模块
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="bundle"></param>
        protected static void CloseModule (string moduleName, Bundle bundle = null) {
            ModuleManager.instance.CloseModule (moduleName, bundle);
        }

        /// <summary>
        /// 启动协程
        /// </summary>
        /// <param name="routine"></param>
        protected void StartCoroutine (IEnumerator routine) {
            CoroutineManager.instance.StartCoroutine (routine);
        }

        /// <summary>
        /// 关闭协程
        /// </summary>
        /// <param name="routine"></param>
        protected void StopCoroutine (IEnumerator routine) {
            CoroutineManager.instance.StopCoroutine (routine);
        }

        /// <summary>
        /// 使操控失效
        /// </summary>
        protected void DisableControl () {
            if (m_eventSystem == null) {
                m_eventSystem = EventSystem.current;
            }
            if (m_eventSystem != null) {
                m_eventSystem.enabled = false;
            }
        }

        /// <summary>
        /// 使操控有效
        /// </summary>
        protected void EnableControl () {
            if (m_eventSystem == null) {
                m_eventSystem = EventSystem.current;
            }
            if (m_eventSystem != null) {
                m_eventSystem.enabled = true;
            }
        }

        private Dictionary<Action,IEnumerator> m_delayRoutines = new Dictionary<Action,IEnumerator>();

        /// <summary>
        /// 延迟执行方法
        /// </summary>
        /// <param name="action"></param>
        /// <param name="time"></param>
        protected void PostDelay(Action action,float time) {
            if (this.m_delayRoutines.ContainsKey(action)) {
                Debug.LogWarning("has delay : " + action.Method.Name);
                return;
            }
            var routine = InvokeDelay(action, time);
            m_delayRoutines.Add(action, routine);
            StartCoroutine(routine);
        }

        protected void ClearAllDelay() {
            foreach(var action in m_delayRoutines.Keys) {
                var routine = this.m_delayRoutines[action];
                this.StopCoroutine(routine);
            }
            this.m_delayRoutines.Clear();
        }

        protected void ClearDelay(Action action) {
            if (this.m_delayRoutines.ContainsKey(action)){
                var routine = this.m_delayRoutines[action];
                this.StopCoroutine(routine);
                this.m_delayRoutines.Remove(action);
            }
        }

        private IEnumerator InvokeDelay(Action action,float time) {
            yield return new WaitForSeconds(time);
            action();
            this.m_delayRoutines.Remove(action);
        }

        public abstract void OnLoad (Bundle bundle);

        public virtual IEnumerator OnLoadAsync(Bundle bundle) {
            yield return null;
        }

        public abstract void OnUnload (Bundle bundle);

        public abstract void OnOpen (Bundle bundle);

        public abstract void OnClose (Bundle bundle);
    }
}