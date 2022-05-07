using UnityEngine;

public class LegionRoadManager : LegionPool<LegionRoad>
{
    private static LegionRoadManager m_instance;

    public static LegionRoadManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                var obj = GameObject.Find(nameof(LegionRoadManager));
                if (obj == null)
                {
                    obj = new GameObject(nameof(LegionRoadManager));
                }
                m_instance = obj.GetComponent<LegionRoadManager>();
                if (m_instance == null)
                {
                    m_instance = obj.AddComponent<LegionRoadManager>();
                }
                var poolObj = Resources.Load<GameObject>("Prefabs/Road");
                m_instance.Init(poolObj);
            }
            return m_instance;
        }
    }
    
    private void Start()
    {
        if (m_instance != null)
        {
            if (m_instance != this)
            {
                Destroy(gameObject);
            }
            return;
        }
        m_instance = this;
        var poolObj = Resources.Load<GameObject>("Prefabs/Road");
        Init(poolObj);
    }
}