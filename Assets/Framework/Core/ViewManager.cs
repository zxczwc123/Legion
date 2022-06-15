// ========================================================
// 描 述：ViewManager.cs 
// 作 者：郑贤春 
// 时 间：2019/06/18 15:08:50 
// 版 本：2018.3.12f1 
// ========================================================
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Framework.Core {
    /// <summary>
    /// 此管理不再管理界面显示关闭，只针对界面进行加载和卸载管理
    /// 对view transform对象进行管理
    /// 界面管理器只针对显示的界面进行 未显示的界面不在此管理当中
    /// </summary>
    public class ViewManager : Manager<ViewManager> {

        /// <summary>
        /// 界面父节点
        /// </summary>
        private RectTransform m_viewRoot;
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
                    m_viewRoot = transform.Find ("Canvas/ViewRoot") as RectTransform;
                }
                return m_viewRoot;
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
                    m_mainCamera = transform.GetComponent<Camera> ();
                }
                return m_mainCamera;
            }
        }

        /// <summary>
        /// 打开的所有
        /// </summary>
        /// <typeparam name="View"></typeparam>
        /// <returns></returns>
        private Dictionary<string, RectTransform> m_viewEntitys;
        /// <summary>
        /// 所有展示的界面
        /// </summary>
        private List<View> m_views;
        /// <summary>
        /// 所有已mask形式展示的界面
        /// </summary>
        private List<View> m_maskViews;

        /// <summary>
        /// 管理器唤醒
        /// </summary>
        protected override void Init () {
            base.Init ();
            m_viewEntitys = new Dictionary<string, RectTransform> ();
            m_views = new List<View> ();
            m_maskViews = new List<View> ();
            m_mask = viewRoot.Find ("Mask") as RectTransform;
        }

        /// <summary>
        /// 加载界面实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public RectTransform LoadViewEntity (string entityName, string tag = null) {
            string entityKey = entityName;
            if (tag != null) {
                entityKey += tag;
            }
            RectTransform entity = null;
            if (m_viewEntitys.ContainsKey (entityKey)) {
                entity = m_viewEntitys[entityKey];
                return entity;
            } else {
                var entityPrefab = ResManager.Instance.Load<GameObject> ("Prefabs/" + entityName);
                if (entityPrefab != null) {
                    entity = Instantiate (entityPrefab.transform as RectTransform);
                    entity.SetParent (viewRoot);
                    entity.anchorMax = Vector2.one;
                    entity.anchorMin = Vector2.zero;
                    entity.localPosition = Vector3.zero;
                    entity.localScale = Vector3.one;
                    entity.sizeDelta = Vector2.zero;
                    m_viewEntitys.Add (entityKey, entity);
                    return entity;
                } else {
                    throw new SystemException (string.Format ("prefab entity named: {0} is not exist!", entityName));
                }
            }
        }

        /// <summary>
        /// 异步加载界面实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IEnumerator LoadViewEntityAsync (string entityName, string tag = null) {
            string entityKey = entityName;
            if (tag != null) {
                entityKey += tag;
            }
            if (m_viewEntitys.ContainsKey (entityKey)) {
                Debug.LogWarning (string.Format ("entityName: {0} is loaded.", entityKey));
                yield break;
            }
            var resourceHolder = new ResourcesHolder<GameObject> ();
            var resourceRequest = ResManager.Instance.LoadAsync<GameObject> ("Prefabs/" + entityName, resourceHolder);
            yield return resourceRequest;
            var entityPrefab = resourceHolder.asset.transform as RectTransform;
            if (entityPrefab != null) {
                var entity = Instantiate (entityPrefab);
                entity.SetParent (viewRoot);
                entity.anchorMax = Vector2.one;
                entity.anchorMin = Vector2.zero;
                entity.localPosition = Vector3.zero;
                entity.localScale = Vector3.one;
                entity.sizeDelta = Vector2.zero;
                m_viewEntitys.Add (entityKey, entity);
            } else {
                throw new SystemException (string.Format ("prefab entity named: {0} is not exist!", entityName));
            }
        }

        /// <summary>
        /// 获取界面实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public RectTransform GetViewEntity (string entityName, string tag = null) {
            RectTransform entity = null;
            string entityKey = entityName;
            if (tag != null) {
                entityKey += tag;
            }
            if (m_viewEntitys.ContainsKey (entityKey)) {
                entity = m_viewEntitys[entityKey];
                return entity;
            } else {
                Debug.LogWarning (string.Format ("entity named: {0} is not loaded!", entityKey));
                return null;
            }
        }

        /// <summary>
        /// 卸载界面预制件
        /// 但一个实体被卸载的时候，同时这个界面被摧毁 将从界面管理中去除
        /// </summary>
        /// <param name="entity"></param>
        public void UnloadViewEntity (string entityName, string tag = null) {
            string entityKey = entityName;
            if (tag != null) {
                entityKey += tag;
            }
            RectTransform entity = null;
            if (m_viewEntitys.ContainsKey (entityKey)) {
                entity = m_viewEntitys[entityKey];
                m_viewEntitys.Remove (entityKey);
                RemoveView (entity);
                Destroy (entity.gameObject);
            } else {
                Debug.LogWarning(string.Format ("entity named: {0} is not loaded!", entityKey));
            }
        }

        /// <summary>
        /// 移除界面，如果界面存在的话
        /// </summary>
        /// <param name="entity"></param>
        private void RemoveView (Transform entity) {
            for (var i = 0; i < m_views.Count; i++) {
                var view = m_views[i];
                if (view.rectTransform == entity) {
                    m_views.Remove (view);
                    break;
                }
            }
            for (var i = 0; i < m_maskViews.Count; i++) {
                var view = m_maskViews[i];
                if (view.rectTransform == entity) {
                    m_views.Remove (view);
                    break;
                }
            }
        }

        /// <summary>
        /// 一个界面被显示
        /// 界面下带mask显示
        /// </summary>
        /// <param name="view"></param>
        public void OnViewShow (View view) {
            if (!this.m_views.Contains (view)) {
                m_views.Add (view);
            }
            if (!this.m_maskViews.Contains (view)) {
                m_maskViews.Add (view);
            }
            view.transform.SetAsLastSibling ();
            m_mask.SetSiblingIndex (view.transform.GetSiblingIndex () - 1);
            m_mask.gameObject.SetActive (true);
            view.gameObject.SetActive (true);
        }

        /// <summary>
        /// 动画显示界面
        /// </summary>
        /// <param name="view"></param>
        /// <param name="animationType"></param>
        /// <param name="duration"></param>
        /// <param name="onCompleted"></param>
        /// <param name="ease"></param>
        /// <param name="delay"></param>
        public void OnViewShow (View view, AnimationShowType animationType, float duration, Action onCompleted, Ease ease, float delay) {
            if (!this.m_views.Contains (view)) {
                m_views.Add (view);
            }
            if (!this.m_maskViews.Contains (view)) {
                m_maskViews.Add (view);
            }
            view.transform.SetAsLastSibling ();
            m_mask.SetSiblingIndex (view.transform.GetSiblingIndex () - 1);
            m_mask.gameObject.SetActive (true);

            if (animationType == AnimationShowType.ShowScale) {
                DoScaleInEdge (view.rectTransform, animationType, duration, onCompleted, ease, delay);
            } else {
                DoMoveInEdge (view.rectTransform, animationType, duration, onCompleted, ease, delay);
            }
        }

        /// <summary>
        /// 缩放入舞台
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="animationType"></param>
        /// <param name="duration"></param>
        /// <param name="onCompleted"></param>
        /// <param name="ease"></param>
        /// <param name="delay"></param>
        private void DoScaleInEdge (RectTransform transform, AnimationShowType animationType, float duration, Action onCompleted, Ease ease, float delay) {
            DOTween.Complete (transform);
            // 从0缩放会导致 scrollview 滚动 所以从 0.1开始缩放
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            transform.gameObject.SetActive (true);
            transform.DOScale (Vector3.one, duration).SetDelay (delay).SetEase (ease).OnComplete (() => {
                if (onCompleted != null) onCompleted ();
            });
        }

        /// <summary>
        /// 移入舞台
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="widget"></param>
        /// <param name="duration"></param>
        /// <param name="delay"></param>
        /// <param name="ease"></param>
        /// <param name="onCompleted"></param>
        private void DoMoveInEdge (RectTransform transform, AnimationShowType animationType, float duration, Action onCompleted, Ease ease, float delay) {
            var rect = viewRoot.rect;
            DOTween.Complete (transform);
            transform.gameObject.SetActive (true);
            var position = transform.localPosition;
            if (animationType == AnimationShowType.ShowFromUp || animationType == AnimationShowType.ShowFromDown) {
                var currentY = transform.localPosition.y;
                var sizeY = transform.rect.height;
                position.y = (rect.height + sizeY) * ((animationType == AnimationShowType.ShowFromUp) ? 0.5f : -0.5f);
                transform.DOLocalMoveY (currentY, duration).SetDelay (delay).SetEase (ease).OnComplete (() => {
                    if (onCompleted != null) onCompleted ();
                });
            } else {
                var currentX = transform.localPosition.x;
                var sizeX = transform.rect.width;
                position.x = (rect.width + sizeX) * ((animationType == AnimationShowType.ShowFromLeft) ? 0.5f : -0.5f);
                transform.DOLocalMoveX (currentX, duration).SetDelay (delay).SetEase (ease).OnComplete (() => {
                    if (onCompleted != null) onCompleted ();
                });
            }
            transform.localPosition = position;
        }

        /// <summary>
        /// 一个界面被显示
        /// 界面下没有带mask显示
        /// </summary>
        /// <param name="view"></param>
        public void OnViewShowAsNoMask (View view) {
            if (!this.m_views.Contains (view)) {
                m_views.Add (view);
            }
            view.transform.SetAsLastSibling ();
            view.gameObject.SetActive (true);
        }

        /// <summary>
        /// 一个界面被隐藏
        /// 如果还有mask显示的界面 mask移动最上面的一个界面下面
        /// 如果没有 mask隐藏
        /// </summary>
        /// <param name="view"></param>
        public void OnViewHide (View view) {
            if (this.m_views.Contains (view)) {
                m_views.Remove (view);
                if (this.m_maskViews.Contains (view)) {
                    m_maskViews.Remove (view);
                    if (this.m_maskViews.Count == 0) {
                        m_mask.gameObject.SetActive(false);
                    } else {
                        var lastView = this.m_maskViews[this.m_maskViews.Count - 1];
                        m_mask.SetSiblingIndex(lastView.transform.GetSiblingIndex());
                    }
                } 
                view.gameObject.SetActive (false);
            } else {
                Debug.LogError (string.Format ("view : {0} is not exist.", view.GetType ()));
            }
        }

        /// <summary>
        /// 动画隐藏界面
        /// </summary>
        /// <param name="view"></param>
        /// <param name="animationType"></param>
        /// <param name="duration"></param>
        /// <param name="onCompleted"></param>
        /// <param name="ease"></param>
        /// <param name="delay"></param>
        public void OnViewHide (View view, AnimationHideType animationType, float duration, Action onCompleted, Ease ease, float delay) {
            if (this.m_views.Contains (view)) {
                m_views.Remove (view);
                if (this.m_maskViews.Contains (view)) {
                    m_maskViews.Remove (view);
                    if (this.m_maskViews.Count == 0) {
                        m_mask.gameObject.SetActive(false);
                    } else {
                        var lastView = this.m_maskViews[this.m_maskViews.Count - 1];
                        m_mask.SetSiblingIndex(lastView.transform.GetSiblingIndex());
                    }
                }
                if (animationType == AnimationHideType.HideScale) {
                    DoScaleOutEdge (view.rectTransform, animationType, duration, onCompleted, ease, delay);
                } else {
                    DoMoveOutEdge (view.rectTransform, animationType, duration, onCompleted, ease, delay);
                }

            } else {
                Debug.LogError (string.Format ("view : {0} is not exist.", view.GetType ()));
            }
        }

        /// <summary>
        /// 缩放入舞台
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="animationType"></param>
        /// <param name="duration"></param>
        /// <param name="onCompleted"></param>
        /// <param name="ease"></param>
        /// <param name="delay"></param>
        private void DoScaleOutEdge (RectTransform transform, AnimationHideType animationType, float duration, Action onCompleted, Ease ease, float delay) {
            transform.DOScale (Vector3.zero, duration).SetDelay (delay).SetEase (ease).OnComplete (() => {
                transform.localScale = Vector3.one;
                transform.gameObject.SetActive(false);
                if (onCompleted != null) onCompleted ();
            });
        }

        /// <summary>
        /// 移入舞台
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="widget"></param>
        /// <param name="duration"></param>
        /// <param name="delay"></param>
        /// <param name="ease"></param>
        /// <param name="onCompleted"></param>
        private void DoMoveOutEdge (RectTransform transform, AnimationHideType animationType, float duration, Action onCompleted, Ease ease, float delay) {
            var rect = viewRoot.rect;
            DOTween.Complete (transform);
            var position = transform.localPosition;
            if (animationType == AnimationHideType.HideToDown || animationType == AnimationHideType.HideToUp) {
                var sizeY = transform.rect.height;
                var currentY = (rect.height + sizeY) * ((animationType == AnimationHideType.HideToUp) ? 0.5f : -0.5f);
                transform.DOLocalMoveY (currentY, duration).SetDelay (delay).SetEase ((Ease) ease).OnComplete (() => {
                    transform.gameObject.SetActive (false);
                    transform.localPosition = position;
                    if (onCompleted != null) onCompleted ();
                });
            } else {
                var sizeX = transform.rect.width;
                var currentX = (rect.width + sizeX) * ((animationType == AnimationHideType.HideToLeft) ? 0.5f : -0.5f);
                transform.DOLocalMoveX (currentX, duration).SetDelay (delay).SetEase ((Ease) ease).OnComplete (() => {
                    transform.gameObject.SetActive (false);
                    transform.localPosition = position;
                    if (onCompleted != null) onCompleted ();
                });
            }
        }

        /// <summary>
        /// 卸载
        /// </summary>
        public override void Dispose () {

        }
    }
}