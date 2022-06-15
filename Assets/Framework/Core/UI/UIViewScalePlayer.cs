using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.UI {
    public class UIViewScalePlayer: MonoBehaviour {

        private bool m_isAwake = false;
        private void OnEnable() {
            //if (m_isAwake) {
                this.transform.localScale = Vector3.zero;
                this.transform.DOScale(Vector3.one, 0.35f);
            //}
        }

        private void Awake() {
            //m_isAwake = true;
            //this.transform.localScale = Vector3.zero;
            //this.transform.DOScale(Vector3.one, 0.35f);
        }

        private void OnDisable() {
            this.transform.localScale = Vector3.zero;
            this.transform.DOScale(Vector3.one, 0.35f);
        }
    }
}
