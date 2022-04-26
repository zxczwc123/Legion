using System;
using System.Collections.Generic;
using UnityEngine;

public class LegionGame : MonoBehaviour
{
    private List<LegionTower> m_towers = new List<LegionTower>();

    private LegionTower m_targetTower;
    
    private List<LegionAI> m_legionAIs = new List<LegionAI>();

    private Vector3[] m_towerPoses = new[]
    {
        new Vector3(0, 0, 0),
        new Vector3(5, -5, 0),
        new Vector3(-5, 5, 0),
        
        new Vector3(5, 5, 0),
        new Vector3(0, 5, 0),
        new Vector3(5, 0, 0),
        new Vector3(-5, -5, 0),
        new Vector3(0, -5, 0),
        new Vector3(-5, 0, 0),
    };
    
    private void Start()
    {
        for (var i = 0; i <m_towerPoses.Length; i++)
        {
            var tower = LegionTowerManager.Instance.Get();
            tower.gameObject.SetActive(true);
            m_towers.Add(tower);
            tower.Legion = i / 3;
            tower.transform.position = m_towerPoses[i];
            tower.OnMouseDownEvent = OnMouseDownEvent;
            tower.OnMouseUpEvent = OnMouseUpEvent;
            tower.OnMouseDragEvent = OnMouseDragEvent;
            tower.OnMouseEnterEvent = OnMouseEnterEvent;
            tower.OnMouseExitEvent = OnMouseExitEvent;
        }
        var aiBlue = gameObject.AddComponent<LegionAI>();
        aiBlue.SetGame(this, (int) LegionType.Blue);
        m_legionAIs.Add(aiBlue);

        var aiRed = gameObject.AddComponent<LegionProAI>();
        aiRed.SetGame(this, (int) LegionType.Red);
        m_legionAIs.Add(aiRed);
    }

    private void OnMouseDownEvent(LegionTower tower)
    {
        foreach (var legionTower in m_towers)
        {
            if (legionTower.Legion == tower.Legion)
            {
                legionTower.SetPathLink(true);
            }
        }
    }
    
    private void OnMouseUpEvent(LegionTower tower)
    {
        foreach (var legionTower in m_towers)
        {
            if (m_targetTower != null && legionTower.IsLink)
            {
                legionTower.Dispatch(m_targetTower);
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
        foreach (var legionTower in m_towers)
        {
            legionTower.UpdatePath();
        }
    }
    
    private void OnMouseEnterEvent(LegionTower tower)
    {
        m_targetTower = tower;
    }
    
    private void OnMouseExitEvent(LegionTower tower)
    {
        m_targetTower = null;
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
        return m_towers;
    }
}