// ========================================================
// 描 述：UpdateAdapter.cs 
// 作 者：郑贤春 
// 时 间：2019/06/21 11:57:16 
// 版 本：2018.3.12f1 
// ========================================================
using System;
using UnityEngine;

namespace Framework.Core.MonoBehaviourAdapter {
    public class MonoBehaviourAdapter: MonoBehaviour {

        public bool IsAwake {
            get;
            private set;
        }

        public bool IsStart {
            get;
            private set;
        }

        public Action awake;

        public Action start;

        public Action onEnable;

        public Action onDisable;

        public Action onDestroy;

        public Action update;

        public Action fixedUpdate;

        public Action lateUpdate;

        private void Awake() {
            IsAwake = true;
            if(awake != null) {
                awake();
            }
        }

        private void Start() {
            IsStart = true;
            if (start != null) {
                start();
            }
        }

        private void OnEnable() {
            if (onEnable != null) {
                onEnable();
            }
        }

        private void OnDisable() {
            if (onDisable != null) {
                onDisable();
            }
        }

        private void OnDestroy() {
            if (onDestroy != null) {
                onDestroy();
            }
        }

        private void Update() {
            if (update != null) {
                update();
            }
        }

        private void LateUpdate() {
            if (lateUpdate != null) {
                lateUpdate();
            }
        }

        private void FixedUpdate() {
            if (fixedUpdate != null) {
                fixedUpdate();
            }
        }
    }
}