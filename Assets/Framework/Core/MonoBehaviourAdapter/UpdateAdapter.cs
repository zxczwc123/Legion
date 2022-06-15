// ========================================================
// 描 述：UpdateAdapter.cs 
// 作 者：郑贤春 
// 时 间：2019/06/21 11:57:16 
// 版 本：2018.3.12f1 
// ========================================================
using System;
using UnityEngine;

namespace Framework.Core.MonoBehaviourAdapter {
    public class UpdateAdapter : MonoBehaviour {
        public Action update;

        public Action fixedUpdate;

        private void Update(){
            if(update != null){
                update();
            }
        }

        private void FixedUpdate(){
            if(fixedUpdate != null){
                fixedUpdate();
            }
        }
    }
}