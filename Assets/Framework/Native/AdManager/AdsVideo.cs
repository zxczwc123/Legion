// ========================================================
// 描 述：AdsVideo.cs 
// 作 者：郑贤春 
// 时 间：2019/06/24 14:11:59 
// 版 本：2018.3.12f1 
// ========================================================

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Ad
{
    /// <summary>
    /// 
    /// </summary>
    public class AdsVideo {
        
        /// <summary>
        /// 当有一个广告加载成功，则加载成功
        /// </summary>
        public Action OnAdLoaded;
        /// <summary>
        /// 广告展示失败条件为全部广告都加载失败则失败
        /// </summary>
        public Action OnAdFailedToLoad;

        public Action OnAdClosedWithReward;

        public Action OnAdClosedWithoutReward;


        public bool IsLoad {
            get{
                return false;
            }
        }

        internal AdsVideo () {

        }

        /// <summary>
        /// 已经加载调用此方法将不产生回调
        /// </summary>
        public void Load () {
            
        }

        /// <summary>
        /// 如果已经有视频展示调用此方法则会无效
        /// </summary>
        public void Show () {
            
        }

        private void HandleOnAdLoaded () {
            MonoBehaviour.print ("HandleAdLoaded event received");
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

        private void HandleOnAdClosedWithReward () {
            MonoBehaviour.print ("HandleAdOpened event received");
            if (OnAdClosedWithReward != null) {
                OnAdClosedWithReward ();
            }
        }

        private void HandleOnAdClosedWithoutReward () {
            MonoBehaviour.print ("HandleAdClosed event received");
            if (OnAdClosedWithoutReward != null) {
                OnAdClosedWithoutReward ();
            }
        }
    }
}