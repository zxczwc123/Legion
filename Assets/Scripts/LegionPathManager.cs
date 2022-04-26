using System.Collections.Generic;
using UnityEngine;


public class LegionPathManager : LegionPool<LegionPath>
{
    private static LegionPathManager m_instance;

    public static LegionPathManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                var obj = GameObject.Find(nameof(LegionPathManager));
                if (obj == null)
                {
                    obj = new GameObject(nameof(LegionPathManager));
                }
                m_instance = obj.GetComponent<LegionPathManager>();
                if (m_instance == null)
                {
                    m_instance = obj.AddComponent<LegionPathManager>();
                }
                var poolObj = GameObject.Find("Path");
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
        var poolObj = GameObject.Find("Path");
        Init(poolObj);
    }
}