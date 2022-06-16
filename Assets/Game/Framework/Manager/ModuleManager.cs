using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Core;
using Game.Common;
using Game.Framework.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Framework
{
    public enum LoadType {
        Sync = 0,
        Async = 1
    }

    public class ModuleManager : MonoSingleton<ModuleManager> {

        public Action OnSceneModuleLoadStart;

        public Action OnSceneModuleLoadEnd;

        private LoadType m_LoadType = LoadType.Sync;

        /// <summary>
        /// 加载的模块表
        /// </summary>
        private Dictionary<string, Module> m_LoadedModuleDict = new Dictionary<string, Module> ();


        private ModuleConfigManager m_ModuleConfig;

        protected override void OnInit()
        {
            m_ModuleConfig = new ModuleConfigManager();
        }

        /// <summary>
        /// 销毁的时候注销所有模块
        /// </summary>
        protected override void OnDestroy() {
            foreach (var loadedModuleName in m_LoadedModuleDict.Keys) {
                var loadModule = m_LoadedModuleDict[loadedModuleName];
                loadModule.OnUnload(null);
            }
            m_LoadedModuleDict.Clear ();
            m_LoadedModuleDict = null;
        }

        public void SetLoadType(LoadType loadType) {
            m_LoadType = loadType;
        }

        /// <summary>
        /// 同步 加载模块
        /// 已经加载的不再加载
        /// 加载的是普通模块 则直接加载
        /// 加载的如果是场景模块 则卸载不是永久的模块 且 不在加载场景所依赖的模块
        /// </summary>
        public void LoadModule (string moduleName, Bundle bundle = null) {
            if (m_LoadedModuleDict.ContainsKey (moduleName)) {
                Debug.LogError (string.Format ("module : {0} is loaded.", moduleName));
                return;
            }
            ModuleInfo info = m_ModuleConfig.GetModuleInfo (moduleName);
            if (info == null) {
                Debug.LogError (string.Format ("module : {0} is not in the config.", moduleName));
                return;
            }
            var module = CreateModule (info, bundle);
            if (module == null) {
                Debug.LogError (string.Format ("module : {0} is not exist.", moduleName));
                return;
            } else {
                if (string.IsNullOrEmpty(module.moduleScene)) {
                    m_LoadedModuleDict.Add(module.moduleName, module);
                    LoadModule(module, bundle);
                } else {
                    if (OnSceneModuleLoadStart != null) OnSceneModuleLoadStart();
                    // 卸载场景模块不需要的模块
                    var needUnloadModules = new List<string>();
                    foreach(var loadedModuleName in m_LoadedModuleDict.Keys){
                        var loadModule = m_LoadedModuleDict[loadedModuleName];
                        if (loadModule.isPermanent) {
                            continue;
                        }
                        if (module.dependModules == null || !Array.Exists(module.dependModules, (x) => x == loadedModuleName)) {
                            needUnloadModules.Add(loadedModuleName);
                        }
                    }
                    for(var i = 0; i < needUnloadModules.Count; i++) {
                        var needUnloadModuleName = needUnloadModules[i];
                        var needUnloadModule = m_LoadedModuleDict[needUnloadModuleName];
                        if(needUnloadModule.dependViews != null) {
                            for(var index = 0;index < needUnloadModule.dependViews.Length; index++) {
                                var unloadView = needUnloadModule.dependViews[index];
                                UIManager.instance.UnloadViewEntity(unloadView);
                            }
                        }
                        needUnloadModule.OnUnload(null);
                        m_LoadedModuleDict.Remove(needUnloadModuleName);
                    }
                    m_LoadedModuleDict.Add(module.moduleName, module);
                    LoadModule(module, bundle);
                }
            }
        }

        /// <summary>
        /// 协程 加载模块
        /// </summary>
        public void LoadModuleAsync(string moduleName, Bundle bundle = null) {
            if (m_LoadedModuleDict.ContainsKey(moduleName)) {
                Debug.LogError(string.Format("module : {0} is loaded.", moduleName));
                return;
            }
            ModuleInfo info = m_ModuleConfig.GetModuleInfo(moduleName);
            if (info == null) {
                Debug.LogError(string.Format("module : {0} is not in the config.", moduleName));
                return;
            }
            var module = CreateModule(info, bundle);
            if (module == null) {
                Debug.LogError(string.Format("module : {0} is not exist.", moduleName));
                return;
            } else {
                if (string.IsNullOrEmpty(module.moduleScene)) {
                    m_LoadedModuleDict.Add(module.moduleName, module);
                    StartCoroutine(LoadModuleProgress(module, bundle));
                } else {
                    if (OnSceneModuleLoadStart != null) OnSceneModuleLoadStart();
                    // 卸载场景模块不需要的模块
                    var needUnloadModules = new List<string>();
                    foreach (var loadedModuleName in m_LoadedModuleDict.Keys) {
                        var loadModule = m_LoadedModuleDict[loadedModuleName];
                        if (loadModule.isPermanent) {
                            continue;
                        }
                        if (module.dependModules == null || !Array.Exists(module.dependModules, (x) => x == loadedModuleName)) {
                            needUnloadModules.Add(loadedModuleName);
                        }
                    }
                    for (var i = 0; i < needUnloadModules.Count; i++) {
                        var needUnloadModuleName = needUnloadModules[i];
                        var needUnloadModule = m_LoadedModuleDict[needUnloadModuleName];
                        if (needUnloadModule.dependViews != null) {
                            for (var index = 0; index < needUnloadModule.dependViews.Length; index++) {
                                var unloadView = needUnloadModule.dependViews[index];
                                UIManager.instance.UnloadViewEntity(unloadView);
                            }
                        }
                        if (needUnloadModule.isOpen) {
                            needUnloadModule.OnClose(null);
                            needUnloadModule.isOpen = false;
                        }
                        needUnloadModule.OnUnload(null);
                        m_LoadedModuleDict.Remove(needUnloadModuleName);
                    }
                    m_LoadedModuleDict.Add(module.moduleName, module);
                    StartCoroutine(LoadModuleProgress(module, bundle));
                }
            }
        }

        /// <summary>
        /// 异步加载模块
        /// </summary>
        /// <param name="module"></param>
        /// <param name="bundle"></param>
        /// <returns></returns>
        private IEnumerator LoadModuleProgress (Module module, Bundle bundle) {
            Debug.Log(string.Format("load module: {0}",module.moduleName));
            // 获取支持的界面信息加载  
            var needLoadViews = new List<string>();
            var needLoadDependModules = new List<Module>();
            if(module.dependModules != null && module.dependModules.Length > 0) {
                for(var i = 0; i < module.dependModules.Length; i++) {
                    var dependModuleName = module.dependModules[i];
                    if (m_LoadedModuleDict.ContainsKey(dependModuleName)) {
                        continue;
                    }
                    ModuleInfo info = m_ModuleConfig.GetModuleInfo(dependModuleName);
                    var dependModule = CreateModule(info, bundle);
                    needLoadDependModules.Add(dependModule);
                    if(dependModule.dependViews != null) {
                        needLoadViews.AddRange(new List<string>(dependModule.dependViews));
                    }
                }
            }
            if(module.dependViews != null) {
                needLoadViews.AddRange(new List<string>(module.dependViews));
            }
            for(var i = 0; i < needLoadViews.Count; i++) {
                var needLoadView = needLoadViews[i];
                yield return UIManager.instance.LoadViewEntityAsync(needLoadView);
            }
            for(var i = 0; i < needLoadDependModules.Count; i++) {
                var needLoadDependModule = needLoadDependModules[i];
                m_LoadedModuleDict.Add(needLoadDependModule.moduleName, needLoadDependModule);
                needLoadDependModule.OnLoad(null);
                needLoadDependModule.isLoaded = true;
            }
            module.OnLoad (bundle);
            module.isLoaded = true;
            if (!string.IsNullOrEmpty(module.moduleScene)) {
                if (OnSceneModuleLoadEnd != null) OnSceneModuleLoadEnd();
            }
        }

        /// <summary>
        /// 加载模块
        /// </summary>
        /// <param name="module"></param>
        /// <param name="bundle"></param>
        /// <returns></returns>
        private void LoadModule(Module module, Bundle bundle) {
            Debug.Log(string.Format("load module: {0}", module.moduleName));
            // 获取支持的界面信息加载  
            var needLoadViews = new List<string>();
            var needLoadDependModules = new List<Module>();
            if (module.dependModules != null && module.dependModules.Length > 0) {
                for (var i = 0; i < module.dependModules.Length; i++) {
                    var dependModuleName = module.dependModules[i];
                    if (m_LoadedModuleDict.ContainsKey(dependModuleName)) {
                        continue;
                    }
                    ModuleInfo info = m_ModuleConfig.GetModuleInfo(dependModuleName);
                    var dependModule = CreateModule(info, bundle);
                    needLoadDependModules.Add(dependModule);
                    if (dependModule.dependViews != null) {
                        needLoadViews.AddRange(new List<string>(dependModule.dependViews));
                    }
                }
            }
            if (module.dependViews != null) {
                needLoadViews.AddRange(new List<string>(module.dependViews));
            }
            for (var i = 0; i < needLoadViews.Count; i++) {
                var needLoadView = needLoadViews[i];
                UIManager.instance.LoadViewEntity(needLoadView);
            }
            for (var i = 0; i < needLoadDependModules.Count; i++) {
                var needLoadDependModule = needLoadDependModules[i];
                m_LoadedModuleDict.Add(needLoadDependModule.moduleName, needLoadDependModule);
                needLoadDependModule.OnLoad(null);
                needLoadDependModule.isLoaded = true;
            }
            module.OnLoad(bundle);
            module.isLoaded = true;
            if (!string.IsNullOrEmpty(module.moduleScene)) {
                if (OnSceneModuleLoadEnd != null) OnSceneModuleLoadEnd();
            }
        }

        /// <summary>
        /// 卸载模块
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="bundle"></param>
        public void UnloadModule (string moduleName, Bundle bundle = null) {
            if (!m_LoadedModuleDict.ContainsKey (moduleName)) {
                Debug.LogError (string.Format ("module : {0} is not loaded.", moduleName));
                return;
            }
            var module = m_LoadedModuleDict[moduleName];
            if (!module.isLoaded) {
                Debug.LogError (string.Format ("module : {0} is loading, can not unload.", moduleName));
                return;
            }
            if (module.isOpen) {
                module.OnClose(bundle);
                module.isOpen = false;
            }
            module.OnUnload (bundle);
            m_LoadedModuleDict.Remove (moduleName);
        }

        /// <summary>
        /// 打开模块
        /// 打开一个未加载的模块 会自动加载模块
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="bundle"></param>
        public void OpenModule (string moduleName, Bundle bundle = null) {
            Module module = null;
            if (!m_LoadedModuleDict.ContainsKey (moduleName)) {
                LoadModule (moduleName, bundle);
            } 
            module = m_LoadedModuleDict[moduleName];
            Debug.Log(string.Format("open module: {0}", module.moduleName));
            module.isOpen = true;
            module.OnOpen(bundle);
        }

        /// <summary>
        /// 打开模块
        /// 打开一个未加载的模块 会自动加载模块
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="bundle"></param>
        public void OpenModuleAsync(string moduleName, Bundle bundle = null) {
            Module module = null;
            if (!m_LoadedModuleDict.ContainsKey(moduleName)) {
                LoadModuleAsync(moduleName, bundle);

                if (m_LoadedModuleDict.ContainsKey(moduleName)) {
                    module = m_LoadedModuleDict[moduleName];
                    StartCoroutine(WaitToOpenModule(module, bundle));
                }
            } else {
                module = m_LoadedModuleDict[moduleName];
                if (module.isLoaded) {
                    Debug.Log(string.Format("open module: {0}", module.moduleName));
                    module.isOpen = true;
                    module.OnOpen(bundle);
                } else {
                    StartCoroutine(WaitToOpenModule(module, bundle));
                }
            }
        }

        private IEnumerator WaitToOpenModule(Module module,Bundle bundle){
            yield return new WaitUntil(()=> module.isLoaded);
            Debug.Log(string.Format("open module: {0}",module.moduleName));
            module.isOpen = true;
            module.OnOpen(bundle);
        }

        /// <summary>
        /// 关闭模块
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="bundle"></param>
        public void CloseModule (string moduleName, Bundle bundle = null) {
            if (!m_LoadedModuleDict.ContainsKey (moduleName)) {
                Debug.LogError (string.Format ("module : {0} is not loaded.", moduleName));
                return;
            }
            var module = m_LoadedModuleDict[moduleName];
            Debug.Log(string.Format("close module: {0}",module.moduleName));
            module.OnClose (bundle);
            module.isOpen = false;
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="module"></param>
        /// <param name="bundle"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        private IEnumerator OnSceneLoad (Module module, Bundle bundle, AsyncOperation operation) {
            while (!operation.isDone) {
                yield return null;
            }
        }

        /// <summary>
        /// 创建模块
        /// </summary>
        /// <param name="moduleInfo"></param>
        /// <param name="bundle"></param>
        /// <returns></returns>
        private Module CreateModule (ModuleInfo moduleInfo, Bundle bundle) {
            var moduleObj = Activator.CreateInstance(moduleInfo.type);
            Module module = moduleObj as Module;
            if (module != null) {
                module.moduleName = moduleInfo.name;
                module.isPermanent = moduleInfo.isPermanent;
                module.moduleScene = moduleInfo.scene;
                module.dependViews = moduleInfo.dependViews;
                module.dependModules = moduleInfo.dependModules;
            }
            return module;
        }

        public bool IsModuleLoad(string moduleName) {
            if (!m_LoadedModuleDict.ContainsKey(moduleName)) {
                return false;
            }
            var module = m_LoadedModuleDict[moduleName];
            return module.isLoaded;
        }


        public bool IsModuleOpen(string moduleName) {
            if (!m_LoadedModuleDict.ContainsKey(moduleName)) {
                return false;
            }
            var module = m_LoadedModuleDict[moduleName];
            return module.isOpen;
        }
    }
}