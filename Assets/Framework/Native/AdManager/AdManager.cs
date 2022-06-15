// ========================================================
// 描 述：AdManager.cs 
// 作 者：郑贤春 
// 时 间：2019/06/24 07:23:10 
// 版 本：2018.3.12f1 
// ========================================================

using System.Collections.Generic;
using  UnityEngine;

namespace Framework.Ad
{
    public class AdManager : MonoBehaviour {

        private static AdManager m_instance;
        public static AdManager Instance {
            get {
                if(m_instance == null){
                    GameObject obj = new GameObject();
                    obj.name = "AdManager";
                    m_instance = obj.AddComponent<AdManager>();
                    m_instance.Init();
                    DontDestroyOnLoad(obj);
                }
                return m_instance;
            }
        }

        private Dictionary<string, AdsBanner> m_adsBannerDict;

        private Dictionary<string, AdsIntersitial> m_adsInterstitialDict;

        private Dictionary<string, AdsVideo> m_adsVideoDict;

        private void Init () {
            m_adsBannerDict = new Dictionary<string, AdsBanner> ();
            m_adsVideoDict = new Dictionary<string, AdsVideo> ();
            m_adsInterstitialDict = new Dictionary<string, AdsIntersitial> ();
        }

        /// <summary>
        /// 获取banner
        /// </summary>
        /// <param name="bannerName"></param>
        /// <returns></returns>
        public AdsBanner GetAdsBanner (string bannerName) {
            if (m_adsBannerDict.ContainsKey (bannerName)) {
                return m_adsBannerDict[bannerName];
            } else {
                var adsBanner = new AdsBanner ();
                m_adsBannerDict.Add (bannerName, adsBanner);
                return adsBanner;
            }
        }

        /// <summary>
        /// 获取banner
        /// </summary>
        /// <param name="bannerName"></param>
        /// <returns></returns>
        public AdsIntersitial GetAdsInterstitial (string interstitialName) {
            if (m_adsInterstitialDict.ContainsKey (interstitialName)) {
                return m_adsInterstitialDict[interstitialName];
            } else {
                var adsIntersitial = new AdsIntersitial ();
                m_adsInterstitialDict.Add (interstitialName, adsIntersitial);
                return adsIntersitial;
            }
        }

        /// <summary>
        /// 获取banner
        /// </summary>
        /// <param name="bannerName"></param>
        /// <returns></returns>
        public AdsVideo GetAdsVideo (string videoName) {
            if (m_adsVideoDict.ContainsKey (videoName)) {
                return m_adsVideoDict[videoName];
            } else {
                var adsVideo = new AdsVideo ();
                m_adsVideoDict.Add (videoName, adsVideo);
                return adsVideo;
            }
        }

        /// <summary>
        /// 摧毁的时候移除引用
        /// </summary>
        private void OnDestroy() {
            m_instance = null;
        }
    }
}