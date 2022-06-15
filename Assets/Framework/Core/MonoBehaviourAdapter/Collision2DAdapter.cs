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

    public class Collision2DAdapter : MonoBehaviour {

        public Action<Collision2D> onCollisionEnter2D;

        public Action<Collision2D> onCollisionExit2D;

        public Action<Collision2D> onCollisionStay2D;

        private void OnCollisionEnter2D (Collision2D collision) {
            if(onCollisionEnter2D != null) onCollisionEnter2D(collision);
        }

        private void OnCollisionExit2D (Collision2D collision) {
            if(onCollisionExit2D != null) onCollisionExit2D(collision);
        }

        private void OnCollisionStay2D (Collision2D collision) {
            if(onCollisionStay2D != null) onCollisionStay2D(collision);
        }
    }
}