using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Native.TTAd {


    public abstract class TTAdManager {

        public static int BANNER_LOADED = 1;
        public static int BANNER_LOADFAILED = 2;
        public static int BANNER_CLICKED = 3;
        public static int BANNER_SHOW = 4;
        public static int BANNER_RENDERFAILED = 5;
        public static int BANNER_RENDERSUCESS = 6;

        public static int FULLVIDEO_LOADFAILED = 7;
        public static int FULLVIDEO_LOADED = 8;
        public static int FULLVIDEO_SHOW = 9;
        public static int FULLVIDEO_BARCLICK = 10;
        public static int FULLVIDEO_CLOSE = 11;
        public static int FULLVIDEO_COMPLETE = 12;
        public static int FULLVIDEO_SKIPPED = 13;

        public static int REWARDVIDEO_LOADFAILED = 14;
        public static int REWARDVIDEO_LOADED = 15;
        public static int REWARDVIDEO_SHOW = 16;
        public static int REWARDVIDEO_BARCLICK = 17;
        public static int REWARDVIDEO_CLOSE = 18;
        public static int REWARDVIDEO_COMPLETE = 19;
        public static int REWARDVIDEO_ERROR = 20;
        public static int REWARDVIDEO_REWARD = 21;
        public static int REWARDVIDEO_SKIPPED = 22;

        public static int INTERSTITIAL_LOADFAILED = 23;
        public static int INTERSTITIAL_LOADED = 24;
        public static int INTERSTITIAL_DISMISSED = 25;
        public static int INTERSTITIAL_CLICKED = 26;
        public static int INTERSTITIAL_SHOW = 27;
        public static int INTERSTITIAL_RENDERFAILED = 28;
        public static int INTERSTITIAL_RENDERSUCCESS = 29;

        private static TTAdManager _adManagerClient;

        public static TTAdManager Instance {
            get {
                if(_adManagerClient == null) {
#if UNITY_EDITOR || UNITY_STANDALONE
                    _adManagerClient = new TTAdManagerClientAnd();
#elif UNITY_IOS
             _adManagerClient = new TTAdManagerClientWin();
#else
             _adManagerClient = new TTAdManagerClientAnd();
#endif
                }
                return _adManagerClient;
            }
        }

        public Action<int> OnBannerAdLoad;

        public Action<int> OnBannerAdAction;

        public Action<int> OnFullVideoAdLoad;

        public Action<int> OnFullVideoAdAction;

        public Action<int> OnRewardVideoAdLoad;

        public Action<int> OnRewardVideoAdAction;

        public Action<int> OnInterstitialAdLoad;

        public Action<int> OnInterstitialAdAction;

        public abstract void Init(string appId);

        public abstract void Unload();

        /// <summary>
        /// 加载成功即显示
        /// </summary>
        /// <param name="codeId"></param>
        public abstract void LoadAdBanner(string codeId);

        public abstract void ShowAdBanner();

        public abstract void ShowAdRewardVideo();

        public abstract void LoadAdRewardVideo(string codeId);

        public abstract void ShowAdFullVideo();

        public abstract void LoadAdFullVideo(string codeId);

        public abstract void ShowAdInterstitialVideo();

        public abstract void LoadAdInterstitialVideo(string codeId);

    }

}
