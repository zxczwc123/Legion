using UnityEngine;
using Random = System.Random;

public class LegionSoldierManager : LegionPool<LegionSoldier>
{
    private static LegionSoldierManager m_instance;

    public static LegionSoldierManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                var obj = GameObject.Find(nameof(LegionSoldierManager));
                if (obj == null)
                {
                    obj = new GameObject(nameof(LegionSoldierManager));
                }
                m_instance = obj.GetComponent<LegionSoldierManager>();
                if (m_instance == null)
                {
                    m_instance = obj.AddComponent<LegionSoldierManager>();
                }
                var poolObj = Resources.Load<GameObject>("Prefabs/Soldier");
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
        var poolObj = Resources.Load<GameObject>("Prefabs/Soldier");
        Init(poolObj);
    }
}