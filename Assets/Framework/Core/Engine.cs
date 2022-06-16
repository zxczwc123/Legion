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

namespace Framework.Core
{
    public class FrameworkEngine : MonoBehaviour
    {
        /// <summary>
        /// 唤醒次数
        /// 用来检测是否多个加载的情况
        /// </summary>
        private static int s_AwakeCount;

        private static FrameworkEngine s_Engine;

        public static FrameworkEngine Instance
        {
            get
            {
                if (s_Engine == null)
                {
                    var engine = GameObject.FindObjectOfType<FrameworkEngine>();
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

        public Action OnAppResume;

        public Action OnAppPause;

        /// <summary>
        /// 启动模块
        /// </summary>
        [SerializeField] private string startModule;

        /// <summary>
        /// 是否热更新
        /// </summary>
        [SerializeField] public bool isHotFix;

        /// <summary>
        /// 是否加载Dll
        /// </summary>
        [SerializeField] internal bool isDLL;

        /// <summary>
        /// 是否Debug
        /// </summary>
        [SerializeField] public bool isDebug;

        /// <summary>
        /// 是否assetBundle资源
        /// </summary>
        [SerializeField] internal bool isAssetBundle;


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
            Debug.Log("FrameworkEngine Awake.");
            if (s_AwakeCount != 0)
            {
                throw new Exception("the multiple FrameworkEngine try to load.");
            }
            s_AwakeCount++;

            if (!isDebug)
            {
                Debug.unityLogger.logEnabled = false;
            }
            else
            {
                Debug.unityLogger.logEnabled = true;
            }

            Initialize();

            StartCoroutine(OpenStartModule());
        }

        private IEnumerator OpenStartModule()
        {
            if (isDLL)
            {
                yield return HotfixManager.Instance.LoadHotFixAssembly();
            }
            if (string.IsNullOrEmpty(startModule))
            {
                throw new Exception("startModule can not be null or empty.");
            }
            ModuleManager.Instance.OpenModule(startModule);
#if UNITY_IOS
            // 此段代码是为了保持代码不被剔除 而作的测试段 ios 存在剔除代码问题
            // 保持长亮
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = 60;
#endif
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
            Debug.Log("FrameworkEngine Destory.");
        }

        private void OnEnable()
        {
            //Debug.Log("OnEnable");
        }

        private void OnDisable()
        {
            //Debug.Log("OnDisable");
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