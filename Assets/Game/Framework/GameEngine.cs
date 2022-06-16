using System;
using System.Collections;
using Framework.Core;
using Game.Common;
using Game.Framework;
using UnityEngine;

namespace Game
{
    public class GameEngine : MonoSingleton<GameEngine>
    {

        private string m_StartModule = "Login";
        
        public void Start()
        {
#if UNITY_IOS
            // 此段代码是为了保持代码不被剔除 而作的测试段 ios 存在剔除代码问题
            // 保持长亮
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = 60;
#endif
            
            Debug.Log($"{nameof(GameEngine)} Start.");
            if (string.IsNullOrEmpty(m_StartModule))
                throw new Exception("startModule can not be null or empty.");
            ModuleManager.instance.OpenModule(m_StartModule);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Debug.Log($"{nameof(Engine)} Destroy.");
        }
    }
}