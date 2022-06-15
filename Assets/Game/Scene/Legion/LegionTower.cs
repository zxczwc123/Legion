using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.Data;
using UnityEngine;

public class LegionTower : MonoBehaviour
{
    public Action<LegionTower> OnMouseDownEvent;
    
    public Action<LegionTower> OnMouseUpEvent;
    
    public Action<LegionTower> OnMouseDragEvent;
    
    public Action<LegionTower> OnMouseEnterEvent;
    
    public Action<LegionTower> OnMouseExitEvent;

    public TowerInfo towerInfo;
    public List<LegionTower> linkTowers = new List<LegionTower>();
    public List<LegionRoad> linkRoads = new List<LegionRoad>();

    public int Id
    {
        set
        {
            towerInfo.Id = value;
            gameObject.name = $"Tower{towerInfo.Id}";
        }
        get
        {
            return towerInfo.Id;
        }
    }

    public int Legion
    {
        get
        {
            return towerInfo.Legion;
        }
        set
        {
            towerInfo.Legion = value;
            Sprite.color = LegionUtil.GetColor(towerInfo.Legion);
            ColorSetter.SetColor((LegionType)towerInfo.Legion);
        }
    }
    
    public int Count
    {
        set
        {
            towerInfo.Count = value;
            Text.text = towerInfo.Count.ToString();
        }
        get
        {
            return towerInfo.Count;
        }
    }

    private int m_maxCount = 20;

    private float m_increaseInterval = 1;
    
    private float m_decreaseInterval = 1;

    private float m_passTime;

    private LegionPath m_path;

    private bool m_linkEnable;

    private bool m_isLink;

    public bool IsLink
    {
        get
        {
            return m_isLink;
        }
    }

    private bool m_isFirstLink;

    private TextMesh m_text;
    protected TextMesh Text
    {
        get
        {
            if (m_text == null)
            {
                m_text = transform.Find("Text").GetComponent<TextMesh>();
            }
            return m_text;
        }
    }
    
    private SpriteRenderer m_sprite;
    protected SpriteRenderer Sprite
    {
        get
        {
            if (m_sprite == null)
            {
                m_sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
            }
            return m_sprite;
        }
    }
    
    private TowerColorSetter m_ColorSetter;
    protected TowerColorSetter ColorSetter
    {
        get
        {
            if (m_ColorSetter == null)
            {
                m_ColorSetter = transform.Find("Entity").GetComponent<TowerColorSetter>();
            }
            return m_ColorSetter;
        }
    }

    private Action<LegionRoad> m_RoadRelease;

    private Func<LegionRoad> m_RoadGet;

    private void Awake()
    {
        if (towerInfo == null)
        {
            towerInfo = new TowerInfo();
        }
        Text.text = towerInfo.Count.ToString();
    }
    
    public void Init(TowerInfo info)
    {
        towerInfo = info;
        Legion = towerInfo.Legion;
        Count = towerInfo.Count;
        gameObject.name = $"Tower{towerInfo.Id}";
        transform.position = towerInfo.position;
    }

    public void InitRoadCreator(Func<LegionRoad> get, Action<LegionRoad> release)
    {
        m_RoadGet = get;
        m_RoadRelease = release;
    }
    
    public void InitRoads(List<LegionTower> towers)
    {
        foreach (var link in towerInfo.Links)
        {
            var tower = towers.Find(x => x.towerInfo.Id == link);
            if (tower != null)
            {
                var road = m_RoadGet();
                road.Link(this, tower);
            }
        }
    }
    
    public void UpdateRoad()
    {
        foreach (var road in linkRoads)
        {
            road.UpdatePosition();
        }
    }

    public void RemoveAllRoads()
    {
        while (linkRoads.Count > 0)
        {
            var road = linkRoads[0];
            var tower0 = road.towers[0];
            var tower1 = road.towers[1];
            tower0.linkRoads.Remove(road);
            tower0.linkTowers.Remove(tower1);
            tower1.linkRoads.Remove(road);
            tower1.linkTowers.Remove(tower0);

            m_RoadRelease(road);
        }
    }

    private void Update()
    {
        if (towerInfo.MaxCount == m_maxCount)
        {
            return;
        }
        m_passTime += Time.deltaTime;
        if (towerInfo.Count > m_maxCount)
        {
            if (m_passTime > m_decreaseInterval)
            {
                m_passTime -= m_decreaseInterval;
                Count--;
                if (towerInfo.Count == m_maxCount)
                {
                    m_passTime = 0;
                }
            }
        }
        else
        {
            if (m_passTime > m_increaseInterval)
            {
                m_passTime -= m_increaseInterval;
                Count++;
                if (towerInfo.Count == m_maxCount)
                {
                    m_passTime = 0;
                }
            }
        }
        
    }

    public void SetPathLink(bool enable)
    {
        m_linkEnable = enable;
    }

    public async void Dispatch(LegionTower target)
    {
        if (target == this)
        {
            return;
        }
        var dispatchCount = Mathf.FloorToInt(towerInfo.Count * 0.5f);
        Count -= dispatchCount;
        var legion = Legion;
        for (var i = 0; i < dispatchCount;i++)
        {
            var soldier = LegionSoldierManager.Instance.Get();
            soldier.gameObject.SetActive(true);
            soldier.transform.position = transform.position;
            soldier.Count = 1;
            soldier.Legion = legion;
            soldier.SetTarget(target);
            await Task.Delay(100);
        }
    }

    private void StartPath()
    {
        if (m_path != null)
        {
            return;
        }
        m_isLink = true;
        m_path = LegionPathManager.Instance.Get();
        m_path.gameObject.SetActive(true);
        m_path.transform.position = transform.position;
        UpdatePath();
    }

    public void EndPath()
    {
        m_isLink = false;
        m_isFirstLink = false;
        if (m_path == null)
        {
            return;
        }
        m_path.gameObject.SetActive(false);
        LegionPathManager.Instance.Release(m_path);
        m_path = null;
    }
    
    private void OnMouseEnter()
    {
        if (m_isFirstLink)
        {
            return;
        }
        if (OnMouseEnterEvent != null) OnMouseEnterEvent(this);
    }

    private void OnMouseExit()
    {
        if (OnMouseExitEvent != null) OnMouseExitEvent(this);
        Debug.Log("OnMouseExit");
        if (!m_linkEnable)
        {
            return;
        }
        if (m_isLink)
        {
            if (m_isFirstLink)
            {
                return;
            }
            EndPath();
        }
        else
        {
            StartPath();
        }
    }
    
    private void OnMouseDown()
    {
        if (OnMouseDownEvent != null) OnMouseDownEvent(this);
        m_isFirstLink = true;
        StartPath();
    }

    private void OnMouseDrag()
    {
        if (OnMouseDragEvent != null) OnMouseDragEvent(this);
    }

    private void OnMouseUp()
    {
        m_isFirstLink = false;
        if (OnMouseUpEvent != null) OnMouseUpEvent(this);
    }

    public void UpdatePath()
    {
        if (!m_linkEnable)
        {
            return;
        }
        if (m_path == null)
        {
            return;
        }
        var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var rayPos = MathTool.IntersectLineAndPlane(worldPos, Camera.main.transform.forward, Vector3.up, Vector3.zero);
        var n = rayPos - transform.position;
        var angle = Vector3.Angle(Vector3.right, n);
        if (n.z > 0)
        {
            angle = -angle;
        }
        m_path.transform.localRotation = Quaternion.Euler(0,angle,0);
        var scale = m_path.transform.localScale;
        scale.x = Vector3.Distance(rayPos, transform.position);
        m_path.transform.localScale = scale;
    }

    public LevelTowerConfig ToLevelConfig()
    {
        var levelConfig = new LevelTowerConfig();
        levelConfig.Id = towerInfo.Id;
        levelConfig.Count = towerInfo.Count;
        levelConfig.Legion = towerInfo.Legion;
        levelConfig.Links = linkTowers.ConvertAll((x) => x.towerInfo.Id).ToArray();
        var pos = gameObject.transform.position;
        levelConfig.Pos = pos;
        levelConfig.TowerId = towerInfo.TowerId;
        return levelConfig;
    }
}