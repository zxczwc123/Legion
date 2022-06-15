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

namespace Framework.I18n
{
    [RequireComponent (typeof (Text))]
    public class I18nText : BaseI18nComponent {
        private Text mText;
        public Text Text {
            get {
                if (mText == null) mText = GetComponent<Text> ();
                return mText;
            }
        }

        protected internal override void I18nUpdate () {
            if (string.IsNullOrEmpty (id)) return;
            var text = I18n.GetValue (id);
            Text.text = text;
        }
    }
}