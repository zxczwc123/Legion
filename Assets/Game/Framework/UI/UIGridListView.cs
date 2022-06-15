// ========================================================
// 描 述：UIGridListView.cs 
// 创 建： 
// 时 间：2020/09/24 16:07:56 
// 版 本：2018.2.20f1 
// ========================================================
using Framework.Core.MonoBehaviourAdapter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Framework.UI {

    /// <summary>
    /// 使用说明 
    /// 支持上下滚动
    /// scrollview content 下挂载gridLayoutGroup 用来是识别列表排列表现
    /// MonoBehaviourAdapter 与 scrollRect 在同一个节点上
    /// item在content的第一个子节点
    /// viewport piovt (0.5,0.5);
    /// content piovt(0.5,1);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UIGridListView<T> where T : UIListElement {

        public RectTransform transform {
            get;
            private set;
        }

        public MonoBehaviourAdapter monoAdpater {
            get;
            private set;
        }
        public ScrollRect ScrollView {
            get;
            private set;
        }
        public RectTransform viewport;
        public RectTransform content;

        private RectTransform itemNode = null;

        private float top = 0;

        private float bottom = 0;

        private float spacingX = 0;

        private float spacingY = 0;

        /// <summary>
        /// 水平方向数量 最小为1
        /// </summary>
        private int horizontalCount = 1;


        private int currentSpawnCount = 0;
        private float lastContentPosY = 0;
        private float itemHeight = 0;
        private float itemWidth = 0;

        /// <summary>
        /// 超出屏幕界限
        /// </summary>
        private float bufferZone;
        /// <summary>
        /// 所有子对象
        /// </summary>
        private List<T> items = new List<T>();
        /// <summary>
        /// 此数据主要用与验证 与data 数量长度的变化
        /// </summary>
        private int dataCount = 0;

        private Action creator;

        private IUIListAdapter<T> adapter;

        private bool _isStart = false;

        private bool _isMoving = false;
        
        public void InitMonoAdapter(MonoBehaviourAdapter adapter) {
            this.transform = adapter.transform as RectTransform;
            this.monoAdpater = adapter;
            this.monoAdpater.awake = Awake;
            this.monoAdpater.start = Start;
            this.monoAdpater.update = Update;

            if (this.monoAdpater.IsAwake) {
                Awake();
            }
            if (this.monoAdpater.IsStart) {
                Start();
            }
        }

        /// <summary>
        /// 设置监听器 由于监听回调item事件
        /// </summary>
        /// <param name="adapter"></param>
        public void SetListAdapter(IUIListAdapter<T> adapter) {
            this.adapter = adapter;
            if (!_isStart) return;
            this.Init();
        }

        /// <summary>
        /// 在挂有horizonta组件没有grid组件时，设置最大水平数，（默认会使用到horizontal的自适应水平排序）；
        /// </summary>
        /// <param name="count"></param>
        public void SetHoriCount(int count){
            this.horizontalCount = count;
        }


        public void Awake() {
            this.InitWidget();
        }

        public void Start() {
            this._isStart = true;
            if (this.adapter == null) return;
            this.Init();
        }

        private void InitWidget() {
            this.ScrollView = this.monoAdpater.GetComponent<ScrollRect>();
            this.ScrollView.onValueChanged.AddListener((arg0 => {
                this._isMoving = true;
            }));
            this.viewport = this.ScrollView.viewport;
            if (viewport == null) {
                throw new Exception(string.Format("viewport null,scrollrect name : {0}", this.ScrollView.transform.name));
            }
            this.content = this.ScrollView.content;
            if (content == null) {
                throw new Exception(string.Format("content null,scrollrect name : {0}", this.ScrollView.transform.name));
            }
            this.viewport.pivot = new Vector2(0.5f, 0.5f);
            this.content.pivot = new Vector2(0.5f, 1f);
            var gridLayout = this.content.GetComponent<GridLayoutGroup>();
            if (gridLayout != null) {
                this.top = gridLayout.padding.top;
                this.bottom = gridLayout.padding.bottom;
                this.spacingX = gridLayout.spacing.x;
                this.spacingY = gridLayout.spacing.y;
                this.horizontalCount = gridLayout.constraintCount;
                gridLayout.enabled = false;
                // 设置gridLayout enable false 会导致 item width height 为0
            }
            this.itemNode = this.content.GetChild(0) as RectTransform;
            if (itemNode == null) {
                throw new Exception(string.Format("itemNode null,scrollrect name : {0}", this.ScrollView.transform.name));
            }
            
            if(gridLayout != null) {
                this.itemHeight = gridLayout.cellSize.y;
                this.itemWidth = gridLayout.cellSize.x;
            } else {
                this.itemHeight = this.itemNode.rect.height;
                this.itemWidth = this.itemNode.rect.width;
            }
            itemNode.sizeDelta = new Vector2(this.itemWidth, this.itemHeight);
            this.lastContentPosY = 0;
            this.itemNode.gameObject.SetActive(false);

            this.bufferZone = this.viewport.rect.height * 0.5f + this.itemHeight * 0.5f;
        }

        /// <summary>
        /// 返回item在ScrollView空间的坐标值
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Vector3 GetPositionInView(RectTransform item) {
            var worldPos = item.position;
            var viewPos = this.viewport.InverseTransformPoint(worldPos);
            return viewPos;
        }

        /// <summary>
        /// 刷新 有两种情况 
        /// 一种是data 的数量可能变化， 数量的bian话在update 中已经自动监听 ，可不对此方法进行刷新
        /// 一种是data的里的数据发生变化
        /// </summary>
        public void Refresh() {
            if (this.adapter == null) {
                return;
            }
            if (!_isStart) {
                return;
            }
            this.InternalRepaint();
        }

        /// <summary>
        /// 初始化绑定数据处理
        /// </summary>
        private void Init() {
            if (this.adapter == null) {
                return;
            }
            this.currentSpawnCount = this.GetSpawnCount();
            var elementCount = this.adapter.GetElementCount();
            var verticalCount = Mathf.Ceil((float)elementCount / this.horizontalCount);
            var sizeDelta = this.content.sizeDelta;
            sizeDelta.y = verticalCount * this.itemHeight + (verticalCount - 1) * this.spacingY + this.top + this.bottom;
            this.content.sizeDelta = sizeDelta;
            this.content.localPosition = new Vector3(0f, this.viewport.rect.height * 0.5f);

            // 此处有个问题 instantiate 导致的事件绑定会一起被复制 目前解决方案提出 第一个 使第一个无绑定事件 即this.itemNode 不参与
            for (var i = 0; i < this.currentSpawnCount; ++i) {
                var tempNode = GameObject.Instantiate(this.itemNode);
                tempNode.SetParent(this.content);
                tempNode.localScale = this.itemNode.localScale;
                tempNode.name = i.ToString();
                var item = this.adapter.OnElementBind(tempNode);
                this.items.Add(item);

                item.index = i;
                item.transform.localPosition = GetItemPosition(i);
                if (i < 0 || i > elementCount - 1) {
                    item.Hide();
                } else {
                    var index = item.index;
                    this.adapter.OnElementUpdate(index, item);
                    item.Show();
                    this.adapter.OnElementRepaint(index, item);
                }
            }
            this.dataCount = elementCount;
        }

        /// <summary>
        /// 数据重新绑定
        /// 列表重新刷新 并置顶
        /// </summary>
        public void Repaint() {
            if (this.adapter == null) {
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

            this.ScrollView.StopMovement();
            var vec3 = new Vector3(0f, this.viewport.rect.height * 0.5f);
            this.content.localPosition = vec3;
            this.lastContentPosY = this.content.localPosition.y;
            for (var i = 0; i < this.items.Count; i++) {
                var item = this.items[i];
                item.index = i;
                var index = item.index;
                item.transform.localPosition = this.GetItemPosition(index);
                if (i < 0 || i > elementCount - 1) {
                    item.Hide();
                } else {
                    item.Show();
                    this.adapter.OnElementUpdate(index, item);
                    this.adapter.OnElementRepaint(index, item);
                }
            }
            this.dataCount = elementCount;
        }



        /// <summary>
        /// 此种情况针对数据刷新 
        /// 可能只是 数量的变化 或者 子数据变化
        /// </summary>
        private void InternalRepaint() {
            if (this.adapter == null) {
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
                    item.transform.localPosition = this.GetItemPosition(index);
                    if (elementCount == 0) {
                        item.Hide();
                    } else {
                        if (index < 0 || index > elementCount - 1) {
                            item.Hide();
                        } else {
                            this.adapter.OnElementUpdate(index, item);
                            item.Show();
                        }
                    }
                }

                this.dataCount = elementCount;
            }
        }

        /// <summary>
        /// 隐藏所有对象
        /// </summary>
        public void Clean() {
            if(this.items == null || this.items.Count == 0) return;
            foreach (var item in this.items) GameObject.DestroyImmediate(item.transform.gameObject);
            this.items.Clear();
        }

        /// <summary>
        /// 获取需要创建的 item数量
        /// 正常为 能显示的 全部数量 加一行  
        /// 但不超过 数据的长度
        /// </summary>
        /// <returns></returns>
        private int GetSpawnCount() {
            var viewHeight = this.viewport.rect.height;
            var spawnCount = (Mathf.Ceil(viewHeight / (this.itemHeight + this.spacingY)) + 1) * this.horizontalCount;
            return (int)spawnCount;
        }

        /// <summary>
        /// update跟新
        /// </summary>
        public void Update() {
            if (!_isStart) {
                return;
            }
            if (this.adapter == null) {
                return;
            }
            if (!_isMoving) {
                return;
            }
            var deltaY = this.content.localPosition.y - this.lastContentPosY;
            if (Mathf.Abs(deltaY) > 0.1) { // 移动中这刷新
                var elementCount = this.adapter.GetElementCount();
                if (elementCount != this.dataCount)
                    this.InternalRepaint();
                var items = this.items;
                var isDown = deltaY < 0;
                // 实际创建项占了多高（即它们的高度累加）
                // 遍历数组，更新item的位置和显示
                for (var i = 0; i < items.Count; ++i) {
                    var item = items[i];
                    var viewPos = this.GetPositionInView(item.rectTransform);
                    if (isDown) {
                        // 如果往下滚动时item已经超出缓冲矩形，且newY未超出content上边界，
                        // 则更新item的坐标（即上移了一个offset的位置），同时更新item的显示内容
                        if (viewPos.y < -this.bufferZone) {

                            var index = item.index - this.items.Count;
                            item.index = index;
                            item.transform.localPosition = this.GetItemPosition(index);
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
                            item.transform.localPosition = this.GetItemPosition(index);
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
            this.lastContentPosY = this.ScrollView.content.localPosition.y;
            this._isMoving = false;
        }

        /// <summary>
        /// 获取item 所在位置的坐标值
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Vector3 GetItemPosition(int index) {
            var verticalIndex = Mathf.Floor((float)index / this.horizontalCount);
            var horizontalIndex = (index < 0 ? (index + this.currentSpawnCount) : index) % this.horizontalCount;
            var x = -(this.horizontalCount * this.itemWidth + (this.horizontalCount - 1) * this.spacingX) * 0.5f + (horizontalIndex + 0.5f) * this.itemWidth + horizontalIndex * this.spacingX;
            var y = -(this.top + this.itemHeight * (0.5f + verticalIndex) + this.spacingY * verticalIndex);
            return new Vector3(x, y);
        }
    }

    public class DefaultUIListAdapter<T, D> : IUIListAdapter<T> where T : DefaultUIListElement<D>, new() {

        private List<D> _data;

        public void SetData(List<D> data) {
            this._data = data;
        }

        public int GetElementCount() {
            if (this._data == null) {
                return 0;
            }
            return this._data.Count;
        }

        public T OnElementBind(RectTransform transform) {
            T element = new T();
            element.Init(transform);
            return element;
        }

        public void OnElementRepaint(int index, T element) {
            element.Repaint();
        }

        public void OnElementUpdate(int index, T element) {
            element.UpdateData(this._data[index]);
        }
    }
    public abstract class DefaultUIListElement<D> : UIListElement {

        public abstract void UpdateData(D data);

        public virtual void Repaint() {

        }
    }

    public abstract class BaseAdapter<T1, T2> : IUIListAdapter<T1> where T1 : UIListElement , new() {
        
        protected List<T2> _data = new List<T2>();

        public void SetData(List<T2> data) => this._data = data;

        public int GetElementCount() {
            return _data.Count;
        }

        public T1 OnElementBind(RectTransform transform) {
            var item = new T1();
            item.Init(transform);
            InitElement(item);
            return item;
        }

        protected virtual void InitElement(T1 element){}

        public abstract void OnElementUpdate(int index, T1 element);

        public virtual void OnElementRepaint(int index, T1 element){}
    }


    public interface IUIListAdapter<T> where T : UIListElement {

        int GetElementCount();

        void OnElementRepaint(int index, T element);

        void OnElementUpdate(int index, T element);

        T OnElementBind(RectTransform transform);
    }

    public abstract class UIListElement {

        public Transform transform;

        public RectTransform rectTransform;

        private int _index = 0;
        public int index {
            get {
                return this._index;
            }
            internal set {
                this._index = value;
            }
        }

        public void Show() {
            this.transform.gameObject.SetActive(true);
        }

        public void Hide() {
            this.transform.gameObject.SetActive(false);
        }

        public void Init(RectTransform transform) {
            this.transform = transform;
            this.rectTransform = transform;
            this.OnBind();
        }

        protected abstract void OnBind();
    }

}
