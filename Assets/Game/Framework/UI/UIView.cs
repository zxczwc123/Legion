// ========================================================
// 描 述：UIView.cs 
// 创 建： 
// 时 间：2020/09/29 11:31:07 
// 版 本：2018.2.20f1 
// ========================================================

using Framework.Core.MonoBehaviourAdapter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Framework.UI
{
    public abstract class UIView<T> : UIView
    {
        private T m_Presenter;

        public UIView(T presenter, RectTransform trans) : base(trans)
        {
            m_Presenter = presenter;
        }
    }

    public abstract class UIView
    {
        public RectTransform rectTransform { private set; get; }

        public Transform transform { private set; get; }

        public GameObject gameObject { private set; get; }


        private List<Action> m_UpdateActions;

        private UpdateAdapter m_UpdateAdapter;

        public UpdateAdapter UpdateAdapter
        {
            get
            {
                if (m_UpdateAdapter == null)
                {
                    m_UpdateAdapter = gameObject.AddComponent<UpdateAdapter>();
                }
                return m_UpdateAdapter;
            }
        }


        public UIView(RectTransform trans)
        {
            if (trans.parent != UIManager.Instance.viewRoot && trans.parent != UIManager.Instance.viewRootMenu)
            {
                throw new Exception(string.Format("{0} not view transform", GetType().Name));
            }
            transform = trans;
            rectTransform = trans;
            gameObject = trans.gameObject;
        }

        public UIView(RectTransform trans, bool isMulti)
        {
            transform = trans;
            rectTransform = trans;
            gameObject = trans.gameObject;
        }

        /// <summary>
        /// 显示界面
        /// </summary>
        public void Show()
        {
            UIManager.Instance.OnViewShow(this);
        }

        /// <summary>
        /// 以没有遮罩的形式显示界面
        /// </summary>
        public void ShowAsNoMask()
        {
            UIManager.Instance.OnViewShowAsNoMask(this);
        }

        /// <summary>
        /// 隐藏界面
        /// </summary>
        public void Hide()
        {
            if (!transform.gameObject.activeSelf)
                return;
            gameObject.SetActive(false);
            UIManager.Instance.OnViewHide(this);
        }

        public void AddUpdate(Action action)
        {
            InitUpdate();
            if (action != null && !m_UpdateActions.Contains(action))
            {
                m_UpdateActions.Add(action);
            }
        }

        public void RemoveUpdate(Action action)
        {
            InitUpdate();
            if (m_UpdateActions.Contains(action))
            {
                m_UpdateActions.Remove(action);
            }
        }

        private void InitUpdate()
        {
            if (m_UpdateActions == null)
            {
                UpdateAdapter.update = Update;
                m_UpdateActions = new List<Action>();
            }
        }

        private void Update()
        {
            for (var i = 0; i < m_UpdateActions.Count; i++)
            {
                m_UpdateActions[i]();
            }
        }

        public void StartCoroutine(IEnumerator routine)
        {
            UpdateAdapter.StartCoroutine(routine);
        }

        public void StopCoroutine(IEnumerator routine)
        {
            UpdateAdapter.StopCoroutine(routine);
        }

        public void StopAllCoroutines()
        {
            UpdateAdapter.StopAllCoroutines();
        }


        private Dictionary<Action, IEnumerator> m_delayRoutines = new Dictionary<Action, IEnumerator>();

        /// <summary>
        /// 延迟执行方法
        /// </summary>
        /// <param name="action"></param>
        /// <param name="time"></param>
        protected void PostDelay(Action action, float time)
        {
            if (m_delayRoutines.ContainsKey(action))
            {
                Debug.LogWarning("has delay : " + action.Method.Name);
                return;
            }
            var routine = InvokeDelay(action, time);
            m_delayRoutines.Add(action, routine);
            StartCoroutine(routine);
        }

        protected void ClearAllDelay()
        {
            foreach (var action in m_delayRoutines.Keys)
            {
                var routine = m_delayRoutines[action];
                StopCoroutine(routine);
            }
            m_delayRoutines.Clear();
        }

        protected void ClearDelay(Action action)
        {
            if (m_delayRoutines.ContainsKey(action))
            {
                var routine = m_delayRoutines[action];
                StopCoroutine(routine);
                m_delayRoutines.Remove(action);
            }
        }

        private IEnumerator InvokeDelay(Action action, float time)
        {
            yield return new WaitForSeconds(time);
            action();
            m_delayRoutines.Remove(action);
        }
    }
}