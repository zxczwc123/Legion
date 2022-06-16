// ========================================================
// 描 述：FrameworkEngine.cs 
// 作 者：郑贤春 
// 时 间：2017/05/01 08:59:32 
// 版 本：5.5.2f1 
// ========================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Framework.Core
{
    public class Engine : MonoBehaviour
    {
        /// <summary>
        /// 唤醒次数
        /// 用来检测是否多个加载的情况
        /// </summary>
        private static int s_AwakeCount;

        private static Engine s_Engine;

        public static Engine instance
        {
            get
            {
                if (s_Engine == null)
                {
                    var engine = GameObject.FindObjectOfType<Engine>();
                    if (engine != null)
                    {
                        engine.Initialize();
                    }
                    else
                    {
                        throw new Exception("FrameworkEngine can not be called when it is not initialized.");
                    }
                }
                return s_Engine;
            }
        }

        public static AppSettings appSettings
        {
            get
            {
                if (s_AppSettings == null)
                {
                    return null;
                }
                return s_AppSettings;
            }
        }

        private static AppSettings s_AppSettings;

        public Action OnAppResume;

        public Action OnAppPause;

        /// <summary>
        /// 是否初始化
        /// </summary>
        private bool m_isInitialized;

        private Queue<Action> m_Actions = new Queue<Action>();

        public void RunOnMain(Action action)
        {
            m_Actions.Enqueue(action);
        }

        public void Update()
        {
            while (m_Actions.Count > 0)
            {
                var action = m_Actions.Dequeue();
                action();
            }
        }

        private void Awake()
        {
            Debug.Log("Engine Awake.");
            if (s_AwakeCount != 0)
            {
                throw new Exception("the multiple FrameworkEngine try to load.");
            }
            s_AwakeCount++;
            Initialize();
            StartCoroutine(StartGameEngine());
        }
        
        private IEnumerator StartGameEngine()
        {
            var handle = Addressables.LoadAssetAsync<AppSettings>("Config/AppSettings");
            yield return handle;
            
            s_AppSettings = handle.Result;
            Debug.unityLogger.logEnabled = appSettings.isDebug;
            if(appSettings.isDLL)
                yield return HotfixManager.Instance.LoadHotFixAssembly();
            HotfixManager.Instance.StartGameEngine();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            if (!m_isInitialized)
            {
                m_isInitialized = true;
                s_Engine = this;
                DontDestroyOnLoad(this);
            }
        }

        private void OnDestroy()
        {
            s_AwakeCount = 0;
            m_isInitialized = false;
            s_Engine = null;
            Debug.Log($"{nameof(Engine)} Destroy.");
        }

        private void OnEnable()
        {
            Debug.Log("OnEnable");
        }

        private void OnDisable()
        {
            Debug.Log("OnDisable");
        }

        private void OnApplicationPause(bool focus)
        {
            //进入程序状态更改为前台
            if (focus)
            {
                Debug.Log("application pause");
                if (OnAppPause != null) OnAppPause();
            }
            else
            {
                //离开程序进入到后台状态
                Debug.Log("application resume");
                if (OnAppResume != null) OnAppResume();
            }
        }
    }
}