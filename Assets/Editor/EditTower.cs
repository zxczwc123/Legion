using System.Collections.Generic;
using DefaultNamespace.Data;
using UnityEngine;

public class EditTower
{
    public GameObject gameObject;
    public TowerInfo towerInfo;
    
    public void Init(TowerInfo info)
    {
        towerInfo = info;
        var towerConfig = ConfigManager.GetDict<TowerConfig>();
        var towerConfigItem = towerConfig[info.TowerId];
        var towerPrefab = Resources.Load<GameObject>(towerConfigItem.Prefab);
        gameObject = GameObject.Instantiate(towerPrefab,LegionEditorWindow.EditRoot.transform);
        gameObject.transform.position = towerInfo.position;
    }
}