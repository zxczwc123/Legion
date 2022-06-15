// ========================================================
// 描 述：UIGridListView.cs 
// 创 建： 
// 时 间：2020/09/24 16:07:56 
// 版 本：2018.2.20f1 
// ========================================================

using Framework.Core.MonoBehaviourAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Framework.UI
{
    /// <summary>
    /// 使用说明 
    /// scrollView content 下挂载gridLayoutGroup 用来是识别列表排列表现
    /// MonoBehaviourAdapter 与 scrollRect 在同一个节点上
    /// item在content的第一个子节点
    /// viewport pivot (0.5,0.5);
    /// content pivot(0,0.5);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UIHorizontalListView<T> where T : UIListElement
    {
        public RectTransform transform { get; private set; }

        public MonoBehaviourAdapter MonoAdapter { get; private set; }
        public ScrollRect ScrollView { get; private set; }

        private RectTransform m_Viewport;
        private RectTransform m_Content;
        private RectTransform m_ItemNode;

        private float m_Left;

        private float m_Right;

        private float m_Spacing;

        private int m_CurrentSpawnCount;
        private float m_LastContentPosX;
        private float m_ItemWidth;

        /// <summary>
        /// 超出屏幕界限
        /// </summary>
        private float m_BufferZone;

        /// <summary>
        /// 所有子对象
        /// </summary>
        private List<T> m_Items = new List<T>();

        /// <summary>
        /// 此数据主要用与验证 与data 数量长度的变化
        /// </summary>
        private int m_DataCount;

        private IUIListAdapter<T> m_Adapter;

        private bool m_IsStart;

        public void InitMonoAdapter(MonoBehaviourAdapter adapter)
        {
            transform = adapter.transform as RectTransform;
            MonoAdapter = adapter;
            MonoAdapter.awake = Awake;
            MonoAdapter.start = Start;
            MonoAdapter.update = Update;

            if (MonoAdapter.IsAwake)
            {
                Awake();
            }
            if (MonoAdapter.IsStart)
            {
                Start();
            }
        }

        /// <summary>
        /// 设置监听器 由于监听回调item事件
        /// </summary>
        /// <param name="adapter"></param>
        public void SetListAdapter(IUIListAdapter<T> adapter)
        {
            m_Adapter = adapter;
            if (!m_IsStart) return;
            Init();
        }


        public void Awake()
        {
            InitWidget();
        }

        public void Start()
        {
            m_IsStart = true;
            if (m_Adapter == null) return;
            Init();
        }

        private void InitWidget()
        {
            ScrollView = MonoAdapter.GetComponent<ScrollRect>();
            m_Viewport = ScrollView.viewport;
            if (m_Viewport == null)
            {
                throw new Exception(string.Format("viewport null,scrollrect name : {0}", ScrollView.transform.name));
            }
            m_Content = ScrollView.content;
            if (m_Content == null)
            {
                throw new Exception(string.Format("content null,scrollrect name : {0}", ScrollView.transform.name));
            }
            var gridLayout = m_Content.GetComponent<HorizontalLayoutGroup>();
            if (gridLayout != null)
            {
                m_Left = gridLayout.padding.left;
                m_Right = gridLayout.padding.right;
                m_Spacing = gridLayout.spacing;
                gridLayout.enabled = false;
            }
            m_ItemNode = m_Content.GetChild(0) as RectTransform;
            if (m_ItemNode == null)
            {
                throw new Exception(string.Format("itemNode null,scrollrect name : {0}", ScrollView.transform.name));
            }
            m_ItemWidth = m_ItemNode.rect.width;
            m_LastContentPosX = 0;
            m_ItemNode.gameObject.SetActive(false);

            m_BufferZone = m_Viewport.rect.width * 0.5f + m_ItemWidth * 0.5f;
        }

        /// <summary>
        /// 返回item在ScrollView空间的坐标值
        /// </summary>
        private Vector3 GetPositionInView(RectTransform item)
        {
            var worldPos = item.position;
            var viewPos = m_Viewport.InverseTransformPoint(worldPos);
            return viewPos;
        }

        /// <summary>
        /// 刷新 有两种情况 
        /// 一种是data 的数量可能变化， 数量的变化在update 中已经自动监听 ，可不对此方法进行刷新
        /// 一种是data的里的数据发生变化
        /// </summary>
        public void Refresh()
        {
            if (m_Adapter == null)
            {
                return;
            }
            if (!m_IsStart)
            {
                return;
            }
            InternalRepaint();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            if (m_Adapter == null)
            {
                return;
            }
            m_CurrentSpawnCount = GetSpawnCount();
            var elementCount = m_Adapter.GetElementCount();
            var sizeDelta = m_Content.sizeDelta;
            sizeDelta.x = elementCount * m_ItemWidth + (elementCount - 1) * m_Spacing + m_Left + m_Right;
            m_Content.sizeDelta = sizeDelta;

            // 此处有个问题 instantiate 导致的事件绑定会一起被复制 目前解决方案提出 第一个 使第一个无绑定事件 即itemNode 不参与
            for (var i = 0; i < m_CurrentSpawnCount; ++i)
            {
                var tempNode = GameObject.Instantiate(m_ItemNode, m_Content);
                tempNode.SetParent(m_Content);
                tempNode.localScale = m_ItemNode.localScale;
                tempNode.name = i.ToString();
                var item = m_Adapter.OnElementBind(tempNode);
                m_Items.Add(item);

                item.index = i;
                item.transform.localPosition = GetItemPosition(i);
                if (i < 0 || i > elementCount - 1)
                {
                    item.Hide();
                }
                else
                {
                    var index = item.index;
                    m_Adapter.OnElementUpdate(index, item);
                    item.Show();
                    m_Adapter.OnElementRepaint(index, item);
                }
            }
            m_DataCount = elementCount;
        }

        /// <summary>
        /// 数据重新绑定
        /// 列表重新刷新 并置顶
        /// </summary>
        public void Repaint()
        {
            if (m_Adapter == null)
            {
                return;
            }
            if (!m_IsStart)
            {
                return;
            }
            var elementCount = m_Adapter.GetElementCount();
            var sizeDelta = m_Content.sizeDelta;
            sizeDelta.x = elementCount * m_ItemWidth + (elementCount - 1) * m_Spacing + m_Left + m_Right;
            m_Content.sizeDelta = sizeDelta;
            ScrollView.StopMovement();
            m_Content.localPosition = new Vector3(-m_Viewport.rect.width * 0.5f + 0.001f, 0f);
            m_LastContentPosX = m_Content.localPosition.x;
            for (var i = 0; i < m_Items.Count; i++)
            {
                var item = m_Items[i];
                item.index = i;
                var index = item.index;
                item.transform.localPosition = GetItemPosition(index);
                if (i < 0 || i > elementCount - 1)
                {
                    item.Hide();
                }
                else
                {
                    m_Adapter.OnElementUpdate(index, item);
                    item.Show();
                    m_Adapter.OnElementRepaint(index, item);
                }
            }
            m_DataCount = elementCount;
        }

        /// <summary>
        /// 此种情况针对数据刷新 
        /// 可能只是 数量的变化 或者 子数据变化
        /// </summary>
        private void InternalRepaint()
        {
            if (m_Adapter == null)
            {
                return;
            }
            var elementCount = m_Adapter.GetElementCount();
            if (elementCount == m_DataCount)
            {
                // 数据内容变化
                for (var i = 0; i < m_Items.Count; i++)
                {
                    var item = m_Items[i];
                    var index = item.index;
                    if (index < 0 || index > elementCount - 1)
                    {
                    }
                    else
                    {
                        m_Adapter.OnElementUpdate(index, item);
                    }
                }
            }
            else
            {
                var sizeDelta = m_Content.sizeDelta;
                sizeDelta.x = elementCount * m_ItemWidth + (elementCount - 1) * m_Spacing + m_Left + m_Right;
                m_Content.sizeDelta = sizeDelta;
                m_Content.localPosition = new Vector3(-m_Viewport.rect.width * 0.5f + 0.001f, 0f);
                // 数据长度变0
                for (var i = 0; i < m_Items.Count; i++)
                {
                    var item = m_Items[i];
                    var index = item.index;
                    item.transform.localPosition = GetItemPosition(index);
                    if (elementCount == 0)
                    {
                        item.Hide();
                    }
                    else
                    {
                        if (index < 0 || index > elementCount - 1)
                        {
                            item.Hide();
                        }
                        else
                        {
                            m_Adapter.OnElementUpdate(index, item);
                            item.Show();
                        }
                    }
                }

                m_DataCount = elementCount;
            }
        }

        /// <summary>
        /// 隐藏所有对象
        /// </summary>
        private void Clean()
        {
            foreach (var item in m_Items) item.Hide();
        }

        /// <summary>
        /// 获取需要创建的 item数量
        /// 正常为 能显示的 全部数量 加一行  
        /// 但不超过 数据的长度
        /// </summary>
        /// <returns></returns>
        private int GetSpawnCount()
        {
            var viewWidth = m_Viewport.rect.width;
            var spawnCount = (Mathf.Ceil(viewWidth / (m_ItemWidth + m_Spacing)) + 1);
            return (int) spawnCount;
        }

        /// <summary>
        /// update跟新
        /// </summary>
        public void Update()
        {
            if (!m_IsStart)
            {
                return;
            }
            if (m_Adapter == null)
            {
                return;
            }
            var deltaX = m_Content.localPosition.x - m_LastContentPosX;
            if (Mathf.Abs(deltaX) > 0.1)
            {
                // 移动中这刷新
                var elementCount = m_Adapter.GetElementCount();
                if (elementCount != m_DataCount)
                    InternalRepaint();
                var items = m_Items;
                var isLeft = deltaX < 0;
                // 实际创建项占了多高（即它们的高度累加）
                // 遍历数组，更新item的位置和显示
                for (var i = 0; i < items.Count; ++i)
                {
                    var item = items[i];
                    var viewPos = GetPositionInView(item.rectTransform);
                    if (isLeft)
                    {
                        // 如果往下滚动时item已经超出缓冲矩形，且newY未超出content上边界，
                        // 则更新item的坐标（即上移了一个offset的位置），同时更新item的显示内容
                        if (viewPos.x < -m_BufferZone)
                        {
                            var index = item.index + items.Count;
                            item.index = index;
                            item.transform.localPosition = GetItemPosition(index);
                            if (index < 0 || index > elementCount - 1)
                            {
                                item.transform.gameObject.SetActive(false);
                            }
                            else
                            {
                                item.transform.gameObject.SetActive(true);
                                m_Adapter.OnElementUpdate(index, item);
                            }
                        }
                    }
                    else
                    {
                        // 如果往上滚动时item已经超出缓冲矩形，且newY未超出content下边界，
                        // 则更新item的坐标（即下移了一个offset的位置），同时更新item的显示内容
                        if (viewPos.x > m_BufferZone)
                        {
                            var index = item.index - items.Count;
                            item.index = index;
                            item.transform.localPosition = GetItemPosition(index);
                            if (index < 0 || index > elementCount - 1)
                            {
                                item.transform.gameObject.SetActive(false);
                            }
                            else
                            {
                                item.transform.gameObject.SetActive(true);
                                m_Adapter.OnElementUpdate(index, item);
                            }
                        }
                    }
                }
            }
            // 更新lastContentPosY和总项数显示
            m_LastContentPosX = ScrollView.content.localPosition.x;
        }

        /// <summary>
        /// 获取item 所在位置的坐标值
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Vector3 GetItemPosition(int index)
        {
            var x = (m_Left + m_ItemWidth * (0.5f + index) + m_Spacing * index);
            var y = 0f;
            return new Vector3(x, y);
        }
    }
}