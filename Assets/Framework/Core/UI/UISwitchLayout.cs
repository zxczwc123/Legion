using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Core.UI {
    public class UISwitchLayout : MonoBehaviour {


        public int currentActiveIndex = 0;

        public List<Transform> currentChildren = null;

        public int activeIndex {
            set {
                if (value < 0) {
                    this.hideAllChildren();
                    return;
                }
                var length = currentChildren.Count;
                if (value >= length) {
                    Debug.LogWarning(string.Format("SwitchLayout ==> child index:{0} is out of range:{1},compenent name: {2}!",value,length,this.name));
                    return;
                }
                this.currentActiveIndex = value;
                for (var i = 0; i < length; i++) {
                    var child = currentChildren[i];
                    child.gameObject.SetActive(i == value);
                }
            }
            get {
                return this.currentActiveIndex;
            }

        }

        public void hideAllChildren() {
            var length = currentChildren.Count;
            for (var i = 0; i < length; i++) {
                var child = currentChildren[i];
                child.gameObject.SetActive(false);
            }
        }

        private void Awake() {
            this.activeIndex = this.currentActiveIndex;
        }
    }
}
