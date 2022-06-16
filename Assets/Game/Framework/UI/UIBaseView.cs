using System;
using System.Collections;
using DG.Tweening;
using Framework.Core;
using Framework.Core.MonoBehaviourAdapter;
using UnityEngine;

namespace Game.Framework.UI
{
    public class UIBaseView
    {
        public RectTransform rectTransform { private set; get; }

        public Transform transform { private set; get; }

        public GameObject gameObject { private set; get; }

        private MonoBehaviour m_CoroutineBehaviour;

        public MonoBehaviour CoroutineBehaviour
        {
            get
            {
                if (m_CoroutineBehaviour == null)
                {
                    m_CoroutineBehaviour = gameObject.AddComponent<UpdateAdapter>();
                }
                return m_CoroutineBehaviour;
            }
        }

        protected UIBaseView()
        {
            
        }

        public UIBaseView(RectTransform rectTransform)
        {
            BindWidget(rectTransform);
        }

        public void BindWidget(RectTransform trans)
        {
            if (trans.parent != UIManager.instance.viewRoot)
            {
                throw new Exception(string.Format("{0} not view transform", GetType().Name));
            }
            transform = trans;
            rectTransform = trans;
            gameObject = trans.gameObject;
        }

        /// <summary>
        /// 显示界面
        /// </summary>
        public void Show()
        {
            UIManager.instance.OnViewShow(this);
        }
        
        /// <summary>
        /// 隐藏界面
        /// </summary>
        public void Hide()
        {
            if (!transform.gameObject.activeSelf)
                return;
            UIManager.instance.OnViewHide(this);
        }

        protected void StartCoroutine(IEnumerator routine)
        {
            CoroutineBehaviour.StartCoroutine(routine);
        }

        protected void StopCoroutine(IEnumerator routine)
        {
            CoroutineBehaviour.StopCoroutine(routine);
        }
    }
}