using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Data;
using UnityEngine;

public class LegionTowerManager : MonoBehaviour
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
            }
            return m_instance;
        }
    }
    
    private Dictionary<string,GameObjectPool> m_pools = new Dictionary<string, GameObjectPool>();
    
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
    }
    
    public LegionTower Create(LevelTowerConfig levelTowerConfig)
    {
        // var towerConfigDict = ConfigManager.GetDict<TowerConfig>();
        // var towerConfig = towerConfigDict[levelTowerConfig.TowerId];
        // var pool = GetPool(towerConfig.Prefab);
        // var towerObj = pool.Get();
        // var tower = towerObj.GetComponent<LegionTower>();
        // var towerInfo = new TowerInfo();
        // towerInfo.Init(levelTowerConfig);
        // tower.Init(towerInfo);
        // return tower;
        return null;
    }

    public void Release(LegionTower tower)
    {
        // var towerConfigDict = ConfigManager.GetDict<TowerConfig>();
        // var towerConfig = towerConfigDict[tower.towerInfo.TowerId];
        // var pool = GetPool(towerConfig.Prefab);
        // pool.Release(tower.gameObject);
    }

    private GameObjectPool GetPool(string prefabName)
    {
        if (m_pools.ContainsKey(prefabName))
        {
            return m_pools[prefabName];
        }
        var pool = new GameObjectPool();
        var prefab = Resources.Load<GameObject>(prefabName);
        pool.Init(prefab);
        return pool;
    }
}