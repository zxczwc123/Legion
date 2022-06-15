// ========================================================
// 描 述：BaseView.cs 
// 创 建： 
// 时 间：2020/09/29 11:31:07 
// 版 本：2018.2.20f1 
// ========================================================

using DG.Tweening;
using Framework.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Common;
using UnityEngine;

namespace Game.Framework.UI {

    public class UIManager : MonoAdapterManager<UIManager> {

        /// <summary>
        /// 界面父节点
        /// </summary>
        private RectTransform m_viewRoot;
        /// <summary>
        /// 界面父节点
        /// </summary>
        private RectTransform m_viewRootMenu;
        /// <summary>
        /// 界面遮罩
        /// </summary>
        private RectTransform m_mask;

        /// <summary>
        /// View节点
        /// </summary>
        /// <value></value>
        public RectTransform viewRoot {
            get {
                if (m_viewRoot == null) {
                    var engineRoot = FrameworkEngine.Instance.gameObject.transform;
                    m_viewRoot = engineRoot.Find("Canvas/Pannel/ViewRoot") as RectTransform;
                }
                return m_viewRoot;
            }
        }

        /// <summary>
        /// MenuView节点
        /// </summary>
        /// <value></value>
        public RectTransform viewRootMenu {
            get {
                if (m_viewRootMenu == null) {
                    var engineRoot = FrameworkEngine.Instance.gameObject.transform;
                    m_viewRootMenu = engineRoot.Find("Canvas/Panel/ViewRootMenu") as RectTransform;
                }
                return m_viewRootMenu;
            }
        }

        private Camera m_mainCamera;

        /// <summary>
        /// View节点
        /// </summary>
        /// <value></value>
        public Camera mainCamera {
            get {
                if (m_mainCamera == null) {
                    var engineRoot = FrameworkEngine.Instance.gameObject.transform;
                    m_mainCamera = engineRoot.Find("MainCamera").GetComponent<Camera>();
                }
                return m_mainCamera;
            }
        }

        /// <summary>
        /// 打开的所有
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, RectTransform> m_ViewEntityDict;
        /// <summary>
        /// 所有展示的界面
        /// </summary>
        private List<UIView> m_views;
        /// <summary>
        /// 所有已mask形式展示的界面
        /// </summary>
        private List<UIView> m_maskViews;


        public UIManager() {
            
        }

        protected override void OnInit() {
            m_ViewEntityDict = new Dictionary<string, RectTransform>();
            m_views = new List<UIView>();
            m_maskViews = new List<UIView>();
            m_mask = viewRoot.Find("Mask") as RectTransform;
            m_mask.sizeDelta = new Vector2(100, 100);
        }

        /// <summary>
        /// 加载界面实体
        /// </summary>
        public RectTransform LoadViewEntity(string entityName, string tag = null) {
            string entityKey = entityName;
            if (tag != null) {
                entityKey += tag;
            }
            RectTransform entity = null;
            if (m_ViewEntityDict.ContainsKey(entityKey)) {
                entity = m_ViewEntityDict[entityKey];
                return entity;
            } else {
                var entityPrefab = ResManager.Instance.Load<GameObject>("Prefabs/" + entityName);
                if (entityPrefab != null) {
                    entity = GameObject.Instantiate(entityPrefab.transform as RectTransform,viewRoot);
                    entity.anchorMax = Vector2.one;
                    entity.anchorMin = Vector2.zero;
                    entity.localPosition = Vector3.zero;
                    entity.localScale = Vector3.one;
                    entity.sizeDelta = Vector2.zero;
                    m_ViewEntityDict.Add(entityKey, entity);
                    return entity;
                } else {
                    throw new SystemException(string.Format("prefab entity named: {0} is not exist!", entityName));
                }
            }
        }

        /// <summary>
        /// 异步加载界面实体
        /// </summary>
        public IEnumerator LoadViewEntityAsync(string entityName, string tag = null) {
            string entityKey = entityName;
            if (tag != null) {
                entityKey += tag;
            }
            if (m_ViewEntityDict.ContainsKey(entityKey)) {
                Debug.LogWarning(string.Format("entityName: {0} is loaded.", entityKey));
                yield break;
            }
            var resourceHolder = new ResourcesHolder<GameObject>();
            var resourceRequest = ResManager.Instance.LoadAsync<GameObject>("Prefabs/" + entityName, resourceHolder);
            yield return resourceRequest;
            var entityPrefab = resourceHolder.asset.transform as RectTransform;
            if (entityPrefab != null) {
                var entity = GameObject.Instantiate(entityPrefab,viewRoot);
                entity.anchorMax = Vector2.one;
                entity.anchorMin = Vector2.zero;
                entity.localPosition = Vector3.zero;
                entity.localScale = Vector3.one;
                entity.sizeDelta = Vector2.zero;
                m_ViewEntityDict.Add(entityKey, entity);
            } else {
                throw new SystemException(string.Format("prefab entity named: {0} is not exist!", entityName));
            }
        }

        /// <summary>
        /// 获取界面实体
        /// </summary>
        public RectTransform GetViewEntity(string entityName, string tag = null) {
            string entityKey = entityName;
            if (tag != null) {
                entityKey += tag;
            }
            if (m_ViewEntityDict.ContainsKey(entityKey)) {
                var entity = m_ViewEntityDict[entityKey];
                return entity;
            } else {
                Debug.LogWarning(string.Format("entity named: {0} is not loaded!", entityKey));
                return null;
            }
        }

        /// <summary>
        /// 卸载界面预制件
        /// 但一个实体被卸载的时候，同时这个界面被摧毁 将从界面管理中去除
        /// </summary>
        public void UnloadViewEntity(string entityName, string tag = null) {
            string entityKey = entityName;
            if (tag != null) {
                entityKey += tag;
            }
            if (m_ViewEntityDict.ContainsKey(entityKey)) {
                var entity = m_ViewEntityDict[entityKey];
                m_ViewEntityDict.Remove(entityKey);
                RemoveView(entity);
                GameObject.Destroy(entity.gameObject);
            } else {
                Debug.LogWarning(string.Format("entity named: {0} is not loaded!", entityKey));
            }
        }

        /// <summary>
        /// 移除界面，如果界面存在的话
        /// </summary>
        /// <param name="entity"></param>
        private void RemoveView(Transform entity) {
            for (var i = 0; i < m_views.Count; i++) {
                var view = m_views[i];
                if (view.rectTransform == entity) {
                    m_views.Remove(view);
                    break;
                }
            }
            for (var i = 0; i < m_maskViews.Count; i++) {
                var view = m_maskViews[i];
                if (view.rectTransform == entity) {
                    m_views.Remove(view);
                    break;
                }
            }
        }

        /// <summary>
        /// 一个界面被显示
        /// 界面下带mask显示
        /// </summary>
        /// <param name="view"></param>
        public void OnViewShow(UIView view) {
            if (!m_views.Contains(view)) {
                m_views.Add(view);
            }
            if (!m_maskViews.Contains(view)) {
                m_maskViews.Add(view);
            }
            view.transform.SetAsLastSibling();
            m_mask.gameObject.SetActive(true);
            view.gameObject.SetActive(true);
        }

        public void OnViewShow(UIBaseView view) {
            view.transform.SetSiblingIndex(view.transform.parent.childCount - 1);
            view.transform.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// 一个界面被显示
        /// 界面下没有带mask显示
        /// </summary>
        /// <param name="view"></param>
        public void OnViewShowAsNoMask(UIView view) {
            if (!m_views.Contains(view)) {
                m_views.Add(view);
            }
            view.transform.SetAsLastSibling();
            view.gameObject.SetActive(true);
        }

        /// <summary>
        /// 一个界面被隐藏
        /// 如果还有mask显示的界面 mask移动最上面的一个界面下面
        /// 如果没有 mask隐藏
        /// </summary>
        /// <param name="view"></param>
        public void OnViewHide(UIView view) {
            if (m_views.Contains(view)) {
                m_views.Remove(view);
                // if (m_maskViews.Contains(view)) {
                //     m_maskViews.Remove(view);
                //     if (m_maskViews.Count == 0) {
                //         m_mask.gameObject.SetActive(false);
                //     } else {
                //         var lastView = m_maskViews[m_maskViews.Count - 1];
                //         m_mask.SetSiblingIndex(lastView.transform.GetSiblingIndex());
                //     }
                // }
                view.gameObject.SetActive(false);
            } else if(m_maskViews.Contains(view)){
                m_maskViews.Remove(view);
            } else {
                Debug.LogWarning(string.Format("view : {0} is not exist.", view.GetType()));
            }
        }

        public void OnViewHide(UIBaseView view) {
            view.transform.gameObject.SetActive(false);
        }

        protected override void OnDestroy() {
            // 正常摧毁应该只有在 热更重启的时候 
            
        }
    }

}
