/*
 * OnionEngine Framework for Unity By wzl
 * -------------------------------------------------------------------
 * Name    : I18n
 * Date    : 2018/10/21
 * Version : v1.0
 * Describe: 
 */
using System;
using System.Collections.Generic;
using Framework.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework.I18n {

    public sealed class I18n : UIBehaviour {

#if UNITY_EDITOR
        [SerializeField]
        private SystemLanguage m_devLanguage = SystemLanguage.English;
#endif
        [SerializeField]
        private LanguageInfo[] m_languageInfos;

        private readonly static Dictionary<SystemLanguage, string> m_languageDict = new Dictionary<SystemLanguage, string> ();

        protected override void Awake () {
            for (var i = 0; i < m_languageInfos.Length; i++) {
                var languageInfo = m_languageInfos[i];
                m_languageDict.Add (languageInfo.language, languageInfo.languagePath);
            }
            mCurrentLanguage = Application.systemLanguage;
#if UNITY_EDITOR
            mCurrentLanguage = m_devLanguage;
#endif
            UpdateSystemLanguage (CurrentLanguage);
        }

        public static I18nLanguage GetLanguage (SystemLanguage language) {
            I18nLanguage lang = null;
            if (language == SystemLanguage.Chinese) {
                language = SystemLanguage.ChineseSimplified;
            }
            if (language == SystemLanguage.ChineseTraditional) {
                language = SystemLanguage.ChineseSimplified;
            }
            string path;
            path = m_languageDict.TryGetValue (language, out path) ? path : path;
            if (!string.IsNullOrEmpty (path)) {
                lang = ResManager.Instance.Load<I18nLanguage> (path);
                if (lang != null) {
                    lang.Init ();
                }
            }
            return lang;
        }

        private static readonly HashSet<BaseI18nComponent> mBaseI18nComponents = new HashSet<BaseI18nComponent> ();

        internal static void OnI18nComponentEnable (BaseI18nComponent component) {
            mBaseI18nComponents.Add (component);
        }

        internal static void OnI18nComponentDisable (BaseI18nComponent component) {
            mBaseI18nComponents.Remove (component);
        }

        private static void UpdateSystemLanguage (SystemLanguage language) {
            var lang = GetLanguage (language);
            if (lang == null) lang = GetLanguage (SystemLanguage.English);
            if (lang != Current) {
                Current = lang;
            }
        }

        private static SystemLanguage mCurrentLanguage = SystemLanguage.Unknown;
        public static SystemLanguage CurrentLanguage {
            get { return mCurrentLanguage; }
            set {
                if (mCurrentLanguage != value)
                    UpdateSystemLanguage (value);
                mCurrentLanguage = value;
            }
        }

        public static I18nLanguage Current {
            get;
            private set;
        }

        public static string GetValue (string key) {
            return Current != null ? Current.GetValue (key) : key;
        }

        public static Font GetFont (string name) {
            return Current != null ? Current.GetFont (name) : null;
        }

        public static Sprite GetSprite (string name) {
            return Current != null ? Current.GetSprite (name) : null;
        }
    }

    [Serializable]
    public class LanguageInfo {
        [SerializeField]
        SystemLanguage m_language;
        public SystemLanguage language {
            get { return m_language; }
        }

        [SerializeField]
        string m_languagePath;
        public string languagePath {
            get { return m_languagePath; }
        }
    }
}