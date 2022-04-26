using UnityEngine;

public class LegionTowerManager : LegionPool<LegionTower>
{
    private static LegionTowerManager m_instance;

    public static LegionTowerManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                var obj = GameObject.Find(nameof(LegionTowerManager));
                if (obj == null)
                {
                    obj = new GameObject(nameof(LegionTowerManager));
                }
                m_instance = obj.GetComponent<LegionTowerManager>();
                if (m_instance == null)
                {
                    m_instance = obj.AddComponent<LegionTowerManager>();
                }
                var poolObj = GameObject.Find("Tower");
                m_instance.Init(poolObj);
            }
            return m_instance;
        }
    }
    
    private void Start()
    {
        if (m_instance != null )
        {
            if (m_instance != this)
            {
                Destroy(gameObject);
            }
            return;
        }
        m_instance = this;
        var poolObj = GameObject.Find("Tower");
        Init(poolObj);
    }
}