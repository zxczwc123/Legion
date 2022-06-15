using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class LegionGame : MonoBehaviour
{
    public static LegionGame instance;
    
    private List<LegionTower> m_Towers = new List<LegionTower>();

    private LegionTower m_TargetTower;
    
    private List<LegionAI> m_LegionAIs = new List<LegionAI>();

    private void Start()
    {
        instance = this;
        var levelConfig = ConfigManager.GetLevelConfig(1);
        if (levelConfig != null)
        {
            foreach (var towerItem in levelConfig.towerConfigs)
            {
                var tower = LegionTowerManager.Instance.Create(towerItem);
                tower.OnMouseDownEvent = OnMouseDownEvent;
                tower.OnMouseDragEvent = OnMouseDragEvent;
                tower.OnMouseEnterEvent = OnMouseEnterEvent;
                tower.OnMouseExitEvent = OnMouseExitEvent;
                tower.OnMouseUpEvent = OnMouseUpEvent;
                tower.InitRoadCreator(LegionRoadManager.Instance.Get,LegionRoadManager.Instance.Release);
                tower.InitRoads(m_Towers);
                m_Towers.Add(tower);
            }
        }

        var aiRed = gameObject.AddComponent<LegionProAI>();
        aiRed.SetGame(this, (int) LegionType.Red);
        m_LegionAIs.Add(aiRed);
    }

    private void OnMouseDownEvent(LegionTower tower)
    {
        foreach (var legionTower in m_Towers)
        {
            if (legionTower.Legion == tower.Legion)
            {
                legionTower.SetPathLink(true);
            }
        }
    }
    
    private void OnMouseUpEvent(LegionTower tower)
    {
        foreach (var legionTower in m_Towers)
        {
            if (m_TargetTower != null && legionTower.IsLink)
            {
                legionTower.Dispatch(m_TargetTower);
            }
            legionTower.SetPathLink(false);
            legionTower.EndPath();
        }
    }

    public void DispatchTower(List<LegionTower> towers, LegionTower targetTower)
    {
        foreach (var tower in towers)
        {
            tower.Dispatch(targetTower);
        }
    }
    
    private void OnMouseDragEvent(LegionTower tower)
    {
        foreach (var legionTower in m_Towers)
        {
            legionTower.UpdatePath();
        }
    }
    
    private void OnMouseEnterEvent(LegionTower tower)
    {
        m_TargetTower = tower;
    }
    
    private void OnMouseExitEvent(LegionTower tower)
    {
        m_TargetTower = null;
    }

    private LegionTower InitTower(GameObject towerObj)
    {
        var tower = towerObj.GetComponent<LegionTower>();
        if (tower == null)
        {
            tower = towerObj.AddComponent<LegionTower>();
        }
        return tower;
    }

    public List<LegionTower> GetTowers()
    {
        return m_Towers;
    }
}