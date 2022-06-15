/*
 * OnionEngine Framework for Unity By wzl
 * -------------------------------------------------------------------
 * Name    : I18nText
 * Date    : 2018/10/31
 * Version : v1.0
 * Describe: 
 */
using UnityEngine;
using UnityEngine.UI;

namespace Framework.I18n {
    public sealed class I18nBitmapFontText : I18nText {
        [SerializeField]
        private string m_fontName;
        protected internal override void I18nUpdate () {
            if (string.IsNullOrEmpty (id)) return;
            var font = I18n.GetFont (m_fontName);
            Text.font = font;
            var text = I18n.GetValue (id);
            Text.text = text;
        }
    }
}