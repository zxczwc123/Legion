using System.Collections.Generic;
using UnityEngine;

public class LegionProAI : LegionAI
{
    protected override void Update()
    {
        if (m_towers == null)
        {
            return;
        }
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
        
        LegionTower targetTower = null;
        float targetDistance = 0;
        for (var i = 0; i < m_towers.Count; i++)
        {
            var tower = m_towers[i];
            var distance = GetDistance(dispatchTowers, tower);
            if(tower.Legion == m_legion) 
                continue;
            if (targetTower == null)
            {
                if (dispatchCount * 0.5f >= tower.Count + 2)
                {
                    targetTower = tower;
                    targetDistance = distance;
                }
            }
            else
            {
                if (dispatchCount * 0.5f < tower.Count + 2)
                {
                    continue;
                }
                if (targetTower.Legion != 0 && tower.Legion == 0)
                {
                    targetTower = tower;
                    targetDistance = distance;
                }
                else if(targetTower.Legion == tower.Legion)
                {
                    if (targetTower.Count > tower.Count)
                    {
                        targetTower = tower;
                        targetDistance = distance;
                    }
                    else if(targetTower.Count > tower.Count)
                    {
                        if (targetDistance > distance)
                        {
                            targetTower = tower;
                            targetDistance = distance;
                        }
                    }
                }
            }
        }
        if (targetTower == null)
            return;
        
        if (dispatchCount * 0.5f >= targetTower.Count + 2)
        {
            m_game.DispatchTower(dispatchTowers,targetTower);
        }
    }

    private float GetDistance(List<LegionTower> towers, LegionTower targetTower)
    {
        float distance = 0;
        foreach (var tower in towers)
        {
            distance += Vector3.Distance(tower.transform.position, targetTower.transform.position);
        }
        return distance;
    }

    public override void OnTowerDispatch(LegionTower tower, LegionTower target)
    {
        
    }
}