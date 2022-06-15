// ========================================================
// 描 述：Trigger2DAdapter.cs 
// 作 者：郑贤春 
// 时 间：2019/06/22 14:36:01 
// 版 本：2018.3.12f1 
// ========================================================
// ========================================================
// 描 述：Collision2DAdapter.cs 
// 作 者：郑贤春 
// 时 间：2019/06/21 11:57:16 
// 版 本：2018.3.12f1 
// ========================================================
using System;
using UnityEngine;

namespace Framework.Core.MonoBehaviourAdapter
{
    [RequireComponent(typeof(Collider2D))]
    [DisallowMultipleComponent]
    public class Trigger2DAdapter : MonoBehaviour {

        public Action<Collider2D> onTriggerEnter2D;

        public Action<Collider2D> onTriggerExit2D;

        public Action<Collider2D> onTriggerStay2D;

        private void OnTriggerEnter2D (Collider2D collider) {
            if(onTriggerEnter2D != null) onTriggerEnter2D(collider);
        }

        private void OnTriggerExit2D (Collider2D collider) {
            if(onTriggerExit2D != null) onTriggerExit2D(collider);
        }

        private void OnTriggerStay2D (Collider2D collider) {
            if(onTriggerStay2D != null) onTriggerStay2D(collider);
        }
    }
}