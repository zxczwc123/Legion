// ========================================================
// 描 述：UIToggleGroup.cs 
// 作 者： 
// 时 间：2020/01/04 11:43:40 
// 版 本：2019.2.1f1 
// ========================================================
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Core.UI {
    /// <summary>
    /// 废弃 ilruntime 不适用
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class UIGridListViewOld : MonoBehaviour {
        [SerializeField]
        public ScrollRect scrollView = null;

        [SerializeField]
        private RectTransform itemNode = null;

        [SerializeField]
        private float top = 0;

        [SerializeField]
        private float bottom = 0;

        [SerializeField]
        private float spacingX = 0;

        [SerializeField]
        private float spacingY = 0;

        /**
         * 水平方向数量 最小为1
         */
        [SerializeField]
        private int horizontalCount = 1;

        private RectTransform view;
        private RectTransform content;
        private int currentSpawnCount = 0;
        private float lastContentPosY = 0;
        private float itemHeight = 0;
        private float itemWidth = 0;

        /**
         * 超出屏幕界限
         */
        private float bufferZone;
        /**
         * 所有子对象
         */
        private List<ListElementOld> items = new List<ListElementOld>();
        /**
         * 此数据主要用与验证 与data 数量长度的变化
         */
        private int dataCount = 0;

        private Action creator;

        private IUIListAdapterOld adapter;

        private bool _isStart = false;

        public ScrollRect ScrollView {
            get {
                if (this.scrollView == null)
                    this.scrollView = this.GetComponent<ScrollRect>();
                return this.scrollView;
            }
        }

        public void Awake() {
            this.initWidget();
        }

        public void Start() {
            this._isStart = true;
            if (this.adapter == null) return;
            this.init();
        }

        private void initWidget() {
            if (this.view != null) return;
            this.content = this.scrollView.content;
            this.view = this.content.parent as RectTransform;
            if (this.horizontalCount < 1) {
                this.horizontalCount = 1;
            }
            this.itemHeight = this.itemNode.rect.height;
            this.itemWidth = this.itemNode.rect.width;
            this.lastContentPosY = 0;
            this.itemNode.gameObject.SetActive(false);

            this.bufferZone = this.view.rect.height * 0.5f + this.itemHeight * 0.5f;
        }

        // 返回item在ScrollView空间的坐标值
        private Vector3 getPositionInView(RectTransform item) {
            var worldPos = item.position;
            var viewPos = this.view.InverseTransformPoint(worldPos);
            return viewPos;
        }

        /**
         * 设置监听器 由于监听回调item事件
         * @param presenter 
         */
        public void SetAdapter(IUIListAdapterOld adapter) {
            this.adapter = adapter;
            if (!_isStart) return;
            this.init();
        }

        /**
         * 刷新 有两种情况 
         * 一种是data 的数量可能变化， 数量的bian话在update 中已经自动监听 ，可不对此方法进行刷新
         * 一种是data的里的数据发生变化
         */
        public void refresh() {
            if(this.adapter == null) {
                return;
            }
            if (_isStart) {
                return;
            }
            this.internalRepaint();
        }

        /**
         * build 初始化绑定数据处理
         * @param data 
         */
        private void init() {
            if(this.adapter == null) {
                return;
            }
            this.currentSpawnCount = this.getSpawnCount();
            var elementCount = this.adapter.GetElementCount();
            var verticalCount = Mathf.Ceil((float)elementCount / this.horizontalCount);
            var sizeDelta = this.content.sizeDelta;
            sizeDelta.y = verticalCount * this.itemHeight + (verticalCount - 1) * this.spacingY + this.top + this.bottom;
            this.content.sizeDelta = sizeDelta;

            // 此处有个问题 instantiate 导致的事件绑定会一起被复制 目前解决方案提出 第一个 使第一个无绑定事件 即this.itemNode 不参与
            for (var i = 0; i < this.currentSpawnCount; ++i) {
                var tempNode = Instantiate(this.itemNode);
                tempNode.SetParent(this.content);
                tempNode.localScale = this.itemNode.localScale;
                tempNode.name = i.ToString();
                var item = this.adapter.OnElementBind(tempNode);
                this.items.Add(item);

                item.index = i;
                item.transform.localPosition = getItemPosition(i);
                if (i < 0 || i > elementCount - 1) {
                    item.hide();
                } else {
                    var index = item.index;
                    this.adapter.OnElementUpdate(index, item);
                    item.show();
                    this.adapter.OnElementRepaint(index, item);
                }
            }
            this.dataCount = elementCount;
        }

        /**
         * 数据重新绑定
         * 列表重新刷新 并置顶
         */
        public void repaint() {
            if(this.adapter == null) {
                return;
            }
            if (!this._isStart) {
                return;
            }
            var elementCount = this.adapter.GetElementCount();
            var verticalCount = Mathf.Ceil((float)elementCount / this.horizontalCount);
            var sizeDelta = this.content.sizeDelta;
            sizeDelta.y = verticalCount * this.itemHeight + (verticalCount - 1) * this.spacingY + this.top + this.bottom;
            this.content.sizeDelta = sizeDelta;
            this.scrollView.StopMovement();
            this.content.localPosition = new Vector3(0f, this.view.rect.height * 0.5f + 0.001f);
            this.lastContentPosY = this.content.position.y;
            for (var i = 0; i < this.items.Count; i++) {
                var item = this.items[i];
                item.index = i;
                var index = item.index;
                item.transform.localPosition = this.getItemPosition(index);
                if (i < 0 || i > elementCount - 1) {
                    item.hide();
                } else {
                    this.adapter.OnElementUpdate(index,item);
                    item.show();
                    this.adapter.OnElementRepaint(index, item);
                }
            }
            this.dataCount = elementCount;
        }

        /**
         * 此种情况针对数据刷新 
         * 可能只是 数量的变化 或者 子数据变化
         */
        private void internalRepaint() {
            if(this.adapter == null) {
                return;
            }
            var elementCount = this.adapter.GetElementCount();
            if (elementCount == this.dataCount) {
                // 数据内容变化
                for (var i = 0; i < this.items.Count; i++) {
                    var item = this.items[i];
                    var index = item.index;
                    if (index < 0 || index > elementCount - 1) {
                    } else {
                        this.adapter.OnElementUpdate(index, item);
                    }
                }
            } else {
                var verticalCount = Mathf.Ceil((float)elementCount / this.horizontalCount);
                var sizeDelta = this.content.sizeDelta;
                sizeDelta.y = verticalCount * this.itemHeight + (verticalCount - 1) * this.spacingY + this.top + this.bottom;
                this.content.sizeDelta = sizeDelta;
                // 数据长度变0
                for (var i = 0; i < this.items.Count; i++) {
                    var item = this.items[i];
                    var index = item.index;
                    item.transform.localPosition = this.getItemPosition(index);
                    if (elementCount == 0) {
                        item.hide();
                    } else {
                        if (index < 0 || index > elementCount - 1) {
                            item.hide();
                        } else {
                            this.adapter.OnElementUpdate(index, item);
                            item.show();
                        }
                    }
                }

                this.dataCount = elementCount;
            }
        }

        /**
         * 隐藏所有对象
         */
        private void clean() {
            foreach (var item in this.items) item.hide();
        }

        /**
         * 获取需要创建的 item数量
         * 正常为 能显示的 全部数量 加一行  
         * 但不超过 数据的长度
         */
        private int getSpawnCount() {
            var viewHeight = this.view.rect.height;
            var spawnCount = (Mathf.Ceil(viewHeight / (this.itemHeight + this.spacingY)) + 1) * this.horizontalCount;
            return (int)spawnCount;
        }

        /**
         * update跟新
         */
        public void Update() {
            if(this.adapter == null) {
                return;
            }
            var deltaY = this.content.localPosition.y - this.lastContentPosY;
            if (Mathf.Abs(deltaY) > 0.1) { // 移动中这刷新
                var elementCount = this.adapter.GetElementCount();
                if (elementCount != this.dataCount)
                    this.internalRepaint();
                var items = this.items;
                var isDown = deltaY < 0;
                // 实际创建项占了多高（即它们的高度累加）
                // 遍历数组，更新item的位置和显示
                for (var i = 0; i < items.Count; ++i) {
                    var item = items[i];
                    var viewPos = this.getPositionInView(item.rectTransform);
                    if (isDown) {
                        // 如果往下滚动时item已经超出缓冲矩形，且newY未超出content上边界，
                        // 则更新item的坐标（即上移了一个offset的位置），同时更新item的显示内容
                        if (viewPos.y < -this.bufferZone) {

                            var index = item.index - this.items.Count;
                            item.index = index;
                            item.transform.localPosition = this.getItemPosition(index);
                            if (index < 0 || index > elementCount - 1) {
                                item.transform.gameObject.SetActive(false);
                                // cc.error("update list hide");
                            } else {
                                item.transform.gameObject.SetActive(true);
                                this.adapter.OnElementUpdate(index, item);
                            }
                        }
                    } else {
                        // 如果往上滚动时item已经超出缓冲矩形，且newY未超出content下边界，
                        // 则更新item的坐标（即下移了一个offset的位置），同时更新item的显示内容
                        if (viewPos.y > this.bufferZone) {
                            var index = item.index + this.items.Count;
                            item.index = index;
                            item.transform.localPosition = this.getItemPosition(index);
                            if (index < 0 || index > elementCount - 1) {
                                item.transform.gameObject.SetActive(false);
                                // cc.error("update list hide");
                            } else {
                                item.transform.gameObject.SetActive(true);
                                this.adapter.OnElementUpdate(index, item);
                            }
                        }
                    }
                }
            }
            // 更新lastContentPosY和总项数显示
            this.lastContentPosY = this.scrollView.content.localPosition.y;
        }

        /**
         * 获取item 所在位置的坐标值
         * @param index 
         */
        private Vector3 getItemPosition(int index) {
            var verticalIndex = Mathf.Floor((float)index / this.horizontalCount);
            var horizontalIndex = (index < 0 ? (index + this.currentSpawnCount) : index) % this.horizontalCount;
            var x = -(this.horizontalCount * this.itemWidth + (this.horizontalCount - 1) * this.spacingX) * 0.5f + (horizontalIndex + 0.5f) * this.itemWidth + horizontalIndex * this.spacingX;
            var y = -(this.top + this.itemHeight * (0.5f + verticalIndex) + this.spacingY * verticalIndex);
            return new Vector3(x, y);
        }
    }

    public interface IUIListAdapterOld {

        int GetElementCount();

        void OnElementRepaint(int index, ListElementOld element);

        void OnElementUpdate(int index, ListElementOld element);

        ListElementOld OnElementBind(RectTransform transform);
    }

    public abstract class ListElementOld {

        public Transform transform;

        public RectTransform rectTransform;

        private int _index = 0;
        public int index {
            get {
                return this._index;
            }
            set {
                this._index = value;
            }
        }

        public void show() {
            this.transform.gameObject.SetActive(true);
        }

        public void hide() {
            this.transform.gameObject.SetActive(false);
        }

        public void Init(RectTransform transform) {
            this.transform = transform;
            this.rectTransform = transform;
            this.onBind();
        }

        protected abstract void onBind();
    }


}

