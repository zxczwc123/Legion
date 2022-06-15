// ========================================================
// 描 述：AdsBanner.cs 
// 作 者：郑贤春 
// 时 间：2019/06/24 14:11:59 
// 版 本：2018.3.12f1 
// ========================================================

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Ad {
    /// <summary>
    /// 插屏广告实现
    /// 目前只集合一个google
    /// </summary>
    public class AdsBanner {

        public Action OnAdLoaded;

        public Action OnAdFailedToLoad;

        public Action OnAdOpened;

        public Action OnAdClosed;

        public Action OnAdLeavingApplication;

        public bool IsLoad {
            private set;
            get;
        }

        public bool IsShow {
            private set;
            get;
        }

        internal AdsBanner () {

        }

        /// <summary>
        /// 已经加载调用此方法将不产生回调
        /// </summary>
        public void Load () {
            if (IsLoad) {
                return;
            }
#if UNITY_EDITOR || STANDALONE
            HandleOnAdLoaded ();
#else
            //if (m_googleAdsBanner != null) {
            //    m_googleAdsBanner.Load ();
            //}
#endif
        }

        public void Show () {
            
        }

        public void Hide () {
            
        }

        private void HandleOnAdLoaded () {
            MonoBehaviour.print ("HandleAdLoaded event received");
            IsLoad = true;
            if (OnAdLoaded != null) {
                OnAdLoaded ();
            }
        }

        private void HandleOnAdFailedToLoad () {
            MonoBehaviour.print ("HandleFailedToReceiveAd event received");
            if (OnAdFailedToLoad != null) {
                OnAdFailedToLoad ();
            }
        }

        private void HandleOnAdOpened () {
            MonoBehaviour.print ("HandleAdOpened event received");
            if (OnAdOpened != null) {
                OnAdOpened ();
            }
        }

        private void HandleOnAdClosed () {
            MonoBehaviour.print ("HandleAdClosed event received");
            if (OnAdClosed != null) {
                OnAdClosed ();
            }
        }

        private void HandleOnAdLeavingApplication () {
            MonoBehaviour.print ("HandleAdLeavingApplication event received");
            if (OnAdLeavingApplication != null) {
                OnAdLeavingApplication ();
            }
        }
    }
}