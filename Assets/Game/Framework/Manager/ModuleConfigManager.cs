// ========================================================
// 描 述：ModuleConfig.cs 
// 作 者：郑贤春 
// 时 间：2017/05/01 10:21:46 
// 版 本：5.5.2f1 
// ========================================================
using Framework.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Framework.Core;
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
        public string type;
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

    public class ModuleConfig {
        public List<ModuleInfo> modules;
    }

    /// <summary>
    /// 模块配置
    /// </summary>
    public class ModuleConfigHandler
    {

        private string m_configPath;
        private Dictionary<string, ModuleInfo> m_configDict = new Dictionary<string, ModuleInfo>();

        private List<ModuleInfo> m_configList;

        public ModuleConfigHandler()
        {
            Init();
        }

        public void Reload()
        {
            this.m_configDict.Clear();
            Init();
        }

        public Dictionary<string, ModuleInfo> GetModuleDict()
        {
            return this.m_configDict;
        }

        public List<ModuleInfo> GetModuleList()
        {
            return this.m_configList;
        }

        public ModuleInfo GetModuleInfo(string moduleName)
        {
            if (this.m_configDict.ContainsKey(moduleName))
            {
                return this.m_configDict[moduleName];
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
            if (!this.m_configDict.ContainsKey(moduleName))
            {
                return false;
            }
            this.m_configDict[moduleName] = module;
            return true;
        }

        public bool AddModule(ModuleInfo module)
        {
            if (module == null || string.IsNullOrEmpty(module.name))
            {
                return false;
            }
            string moduleName = module.name;
            if (this.m_configDict.ContainsKey(moduleName))
            {
                return false;
            }
            this.m_configDict.Add(moduleName, module);
            this.m_configList.Add(module);
            return true;
        }

        public bool RomoveModule(string moduleName)
        {
            if (!this.m_configDict.ContainsKey(moduleName))
            {
                return false;
            }
            ModuleInfo module = this.m_configDict[moduleName];
            this.m_configDict.Remove(moduleName);
            this.m_configList.Remove(module);
            return true;
        }

        public bool Save()
        {
            return CsvUtility.Save<ModuleInfo>(this.m_configPath, this.m_configList);
        }

        private void Init()
        {
            var configAsset = ResManager.Instance.Load<TextAsset>("Config/ModuleConfig");
            string txt = configAsset.text;
            var moduleConfig = LitJson.JsonMapper.ToObject<ModuleConfig>(txt);
            this.m_configList = moduleConfig.modules;
            if (m_configList == null)
                return;
            for (int i = 0; i < m_configList.Count; i++)
            {
                ModuleInfo config = m_configList[i];
                if (this.m_configDict.ContainsKey(config.name))
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
                this.m_configDict.Add(config.name, config);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetTxtInStreaming(string filename) {
            try {

                if (Application.platform == RuntimePlatform.Android) {
                    WWW www = new WWW(filename);
                    while (!www.isDone) { }
                    string context = www.text;
                    return context;
                } else if (Application.platform == RuntimePlatform.IPhonePlayer) {
                    if (string.IsNullOrEmpty(filename)) throw new ArgumentException("Filename can't be null or empty!");
                    if (!File.Exists(filename)) throw new ArgumentException("File not exist!");
                    using (StreamReader sr = new StreamReader(File.OpenRead(filename), Encoding.UTF8)) {
                        string context = sr.ReadToEnd();
                        return context;
                    }
                } else {
                    if (string.IsNullOrEmpty(filename)) throw new ArgumentException("Filename can't be null or empty!");
                    if (!File.Exists(filename)) throw new ArgumentException("File not exist!");
                    using (StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open), Encoding.UTF8)) {
                        string context = sr.ReadToEnd();
                        return context;
                    }
                }
            } catch (Exception e) {
                Debug.LogError(filename + "Unknown Exception :" + e.ToString());
                return null;
            }
        }
    }
}
