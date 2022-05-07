using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Data;
using Editor;
using UnityEngine;

public class EditTower
{
    public GameObject gameObject;
    public TowerInfo towerInfo;
    public List<EditTower> linkTowers = new List<EditTower>();
    public List<EditRoad> linkRoads = new List<EditRoad>();
    
    public void Init(TowerInfo info)
    {
        towerInfo = info;
        var towerConfig = ConfigManager.GetDict<TowerConfig>();
        var towerConfigItem = towerConfig[info.TowerId];
        var towerPrefab = Resources.Load<GameObject>(towerConfigItem.Prefab);
        gameObject = GameObject.Instantiate(towerPrefab,LegionEditorWindow.EditRoot.transform);
        gameObject.name = $"Tower{towerInfo.Id}";
        gameObject.transform.position = towerInfo.position;
    }

    public void InitRoads(List<EditTower> towers)
    {
        foreach (var link in towerInfo.Links)
        {
            var tower = towers.Find(x => x.towerInfo.Id == link);
            if (tower != null)
            {
                var road = RoadManager.Create();
                road.Link(this, tower);
            }
        }
    }

    public void SetId(int id)
    {
        towerInfo.Id = id;
        gameObject.name = $"Tower{towerInfo.Id}";
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
            
            GameObject.DestroyImmediate(road.gameObject);
        }
    }

    public LevelConfig ToLevelConfig()
    {
        var levelConfig = new LevelConfig();
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