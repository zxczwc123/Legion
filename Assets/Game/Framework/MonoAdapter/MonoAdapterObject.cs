// ========================================================
// 描 述：MonoAdapterObject.cs 
// 创 建： 
// 时 间：2020/10/15 10:53:42 
// 版 本：2018.2.20f1 
// ========================================================
using Framework.Core.MonoBehaviourAdapter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Common {
    public abstract class MonoAdapterObject {

        public Transform transform {
            get;
            private set;
        }
        public GameObject gameObject {
            get;
            private set;
        }

        protected MonoBehaviourAdapter m_Adapter;

        public void Init(Transform trans) {
            transform = trans;
            gameObject = trans.gameObject;
            m_Adapter = trans.GetComponent<MonoBehaviourAdapter>();
            if (m_Adapter == null) {
                m_Adapter = trans.gameObject.AddComponent<MonoBehaviourAdapter>();
            }
            OnInit();
        }

        protected abstract void OnInit();

        public void StartCoroutine(IEnumerator routine) {
            m_Adapter.StartCoroutine(routine);
        }

        public void StopCoroutine(IEnumerator routine) {
            m_Adapter.StopCoroutine(routine);
        }

        public void StopAllCoroutines() {
            m_Adapter.StopAllCoroutines();
        }
    }
}
