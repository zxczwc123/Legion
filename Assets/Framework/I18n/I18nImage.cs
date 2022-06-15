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
    [RequireComponent (typeof (Image))]
    public class I18nImage : BaseI18nComponent {
        private Image mImage;
        public Image Image {
            get {
                if (mImage == null) mImage = GetComponent<Image> ();
                return mImage;
            }
        }

        protected internal override void I18nUpdate () {
            if (string.IsNullOrEmpty (id)) return;
            var spr = I18n.GetSprite (id);
            Image.sprite = spr;
            Image.SetNativeSize ();
        }
    }
}