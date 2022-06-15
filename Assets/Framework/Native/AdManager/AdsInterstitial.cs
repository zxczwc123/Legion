// ========================================================
// 描 述：AdsInterstitial.cs 
// 作 者：郑贤春 
// 时 间：2019/06/24 15:17:50 
// 版 本：2018.3.12f1 
// ========================================================

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Ad
{
    /// <summary>
    /// 插页广告 目前只集合google
    /// </summary>
    public class AdsIntersitial {
        /// <summary>
        /// 当有一个广告加载成功，则加载成功
        /// </summary>
        public Action OnAdLoaded;
        /// <summary>
        /// 广告展示失败条件为全部广告都加载失败则失败
        /// </summary>
        public Action OnAdFailedToLoad;

        public Action OnAdOpened;

        public Action OnAdClosed;

        public Action OnAdLeavingApplication;


        //private GoogleAdsInterstitial m_googleAdsInterstitial;

        public bool IsLoad {
            get{
                return false;
            }
        }

        internal AdsIntersitial () {

        }

        /// <summary>
        /// 已经加载调用此方法将不产生回调
        /// </summary>
        public void Load () {
            if (IsLoad) {
                return;
            }
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

        private void HandleOnAdLeavingApplication(){

        }
    }
}