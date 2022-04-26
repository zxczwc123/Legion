using System.Collections.Generic;
using UnityEngine;

public class LegionAI : MonoBehaviour
{
    protected int m_legion;

    protected List<LegionTower> m_towers;

    protected LegionGame m_game;

    public void SetGame(LegionGame game,int legion)
    {
        m_game = game;
        m_legion = legion;
        m_towers = m_game.GetTowers();
    }

    protected virtual void Update()
    {
        if (m_towers == null)
        {
            return;
        }
        LegionTower targetTower = null;
        for (var i = 0; i < m_towers.Count; i++)
        {
            var tower = m_towers[i];
            if(tower.Legion == m_legion) 
                continue;
            if (targetTower == null)
            {
                targetTower = tower;
            }
            else
            {
                if (targetTower.Count > tower.Count)
                {
                    targetTower = tower;
                }
            }
        }
        if (targetTower == null)
            return;
        List<LegionTower> dispatchTowers = new List<LegionTower>();
        var dispatchCount = 0;
        for (var i = 0; i < m_towers.Count; i++)
        {
            var tower = m_towers[i];
            if(tower.Legion != m_legion) 
                continue;
            dispatchTowers.Add(tower);
            dispatchCount += tower.Count;
        }
        if (dispatchCount > targetTower.Count)
        {
            m_game.DispatchTower(dispatchTowers,targetTower);
        }
    }

    public virtual void OnTowerDispatch(LegionTower tower, LegionTower target)
    {
        
    }
}