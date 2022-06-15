/*
 * OnionEngine Framework for Unity By wzl
 * -------------------------------------------------------------------
 * Name    : I18nLanguage
 * Date    : 2018/10/21
 * Version : v1.0
 * Describe: 
 */
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.I18n {

    [CreateAssetMenu (menuName = "Framework/i18n/CreateLanguage")]
    public sealed class I18nLanguage : ScriptableObject {
 
        [SerializeField]
        private SystemLanguage m_language = SystemLanguage.ChineseSimplified;
        public SystemLanguage language {
            get { return m_language; }
        }

        [SerializeField]
        private TextAsset m_textAsset;
        public TextAsset textAsset {
            get { return m_textAsset; }
        }

        [SerializeField]
        private Font[] m_fonts;

        [SerializeField]
        private Sprite[] m_sprites;

        private readonly Dictionary<string, string> mValueDict = new Dictionary<string, string> ();
        private readonly Dictionary<string, Font> mFontDict = new Dictionary<string, Font> ();
        private readonly Dictionary<string, Sprite> mSpriteDict = new Dictionary<string, Sprite> ();

        internal void Init () {
            
            if (m_textAsset != null) {
                var content = m_textAsset.text;
                content = content.Replace ("\r", string.Empty);
                try {
                    string[] lines = content.Split ("\n".ToCharArray ());
                    for (int i = 0; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line.Contains ("//")) continue;
                        var index = line.IndexOf ("=", StringComparison.Ordinal);
                        if (index <= 0) continue;
                        var startIdx = index + 1;
                        string key = line.Substring (0, index).Trim ();
                        string value = line.Substring (startIdx, line.Length - startIdx).Trim ();
                        mValueDict[key] = value;
                    }
                } catch (Exception e) {
                    Debug.Log(e.ToString());
                }
            }
            if (m_fonts != null && m_fonts.Length != 0) {
                foreach (var font in m_fonts)
                    mFontDict[font.name] = font;
            }
            if (m_sprites != null && m_sprites.Length != 0) {
                foreach (var spr in m_sprites)
                    mSpriteDict[spr.name] = spr;
            }
        }

        public string GetValue (string key) {
            string value = null;
            mValueDict.TryGetValue (key, out value);
            return value;
        }

        public Font GetFont (string name) {
            Font font = null;
            mFontDict.TryGetValue (name, out font);
            return font;
        }

        public Sprite GetSprite (string name) {
            Sprite spr = null;
            mSpriteDict.TryGetValue (name, out spr);
            return spr;
        }
    }
}