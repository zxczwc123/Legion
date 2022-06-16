// ========================================================
// 描 述：ModuleConfig.cs 
// 作 者：郑贤春 
// 时 间：2017/05/01 10:21:46 
// 版 本：5.5.2f1 
// ========================================================
using System;
using System.Collections.Generic;
using Game.Config;
using UnityEngine;

namespace Game.Framework
{
    /// <summary>
    /// 模块信息
    /// </summary>
    public class ModuleInfo
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public string name;
        /// <summary>
        /// 模块类全名
        /// </summary>
        public Type type;
        /// <summary>
        /// 是否永久模块
        /// </summary>
        public bool isPermanent;
        /// <summary>
        /// 模块场景 
        /// 是否场景模块 无设置表示普通模块
        /// </summary>
        public string scene;
        /// <summary>
        /// 
        /// </summary>
        public string[] dependViews;
        /// <summary>
        /// 
        /// </summary>
        public string[] dependModules;
    }

    /// <summary>
    /// 模块配置
    /// </summary>
    public class ModuleConfigManager
    {

        private Dictionary<string, ModuleInfo> m_ConfigDict = new Dictionary<string, ModuleInfo>();

        private List<ModuleInfo> m_ConfigList;

        public ModuleConfigManager()
        {
            Init();
        }

        public void Reload()
        {
            m_ConfigDict.Clear();
            Init();
        }

        public Dictionary<string, ModuleInfo> GetModuleDict()
        {
            return m_ConfigDict;
        }

        public List<ModuleInfo> GetModuleList()
        {
            return m_ConfigList;
        }

        public ModuleInfo GetModuleInfo(string moduleName)
        {
            if (m_ConfigDict.ContainsKey(moduleName))
            {
                return m_ConfigDict[moduleName];
            }
            return null;
        }

        public bool SetModuleInfo(ModuleInfo module)
        {
            if (module == null || string.IsNullOrEmpty(module.name))
            {
                return false;
            }
            string moduleName = module.name;
            if (!m_ConfigDict.ContainsKey(moduleName))
            {
                return false;
            }
            m_ConfigDict[moduleName] = module;
            return true;
        }

        public bool AddModule(ModuleInfo module)
        {
            if (module == null || string.IsNullOrEmpty(module.name))
            {
                return false;
            }
            string moduleName = module.name;
            if (m_ConfigDict.ContainsKey(moduleName))
            {
                return false;
            }
            m_ConfigDict.Add(moduleName, module);
            m_ConfigList.Add(module);
            return true;
        }

        public bool RemoveModule(string moduleName)
        {
            if (!m_ConfigDict.ContainsKey(moduleName))
            {
                return false;
            }
            ModuleInfo module = m_ConfigDict[moduleName];
            m_ConfigDict.Remove(moduleName);
            m_ConfigList.Remove(module);
            return true;
        }


        private void Init()
        {
            m_ConfigList = new List<ModuleInfo>(ModuleConfig.Value);
            for (int i = 0; i < m_ConfigList.Count; i++)
            {
                ModuleInfo config = m_ConfigList[i];
                if (m_ConfigDict.ContainsKey(config.name))
                {
                    try
                    {
                        throw new Exception(string.Format("module ：{0} is defined multiple times.", config.name));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                        continue;
                    }
                }
                m_ConfigDict.Add(config.name, config);
            }
        }
    }
}
