using DefaultNamespace.Data;
using UnityEngine;

namespace Editor
{
    public class TowerManager
    {
        public static LegionTower CreateTower(LevelTowerConfig towerItem)
        {
            var towerConfigDict = ConfigManager.GetDict<TowerConfig>();
            var towerConfig = towerConfigDict[towerItem.TowerId];
            var tower = CreateTowerInstance(towerConfig);
            var towerInfo = new TowerInfo();
            towerInfo.Init(towerItem);
            tower.Init(towerInfo);
            return tower;
        }

        public static LegionTower CreateTower(TowerConfig towerConfig)
        {
            var tower = CreateTowerInstance(towerConfig);
            var towerInfo = new TowerInfo();
            var levelConfig = new LevelTowerConfig();
            levelConfig.Id = 0;
            levelConfig.TowerId = towerConfig.Id;
            levelConfig.Pos = Vector3.zero;
            towerInfo.Init(levelConfig);
            tower.Init(towerInfo);
            return tower;
        }

        public static LegionTower CreateTowerInstance(TowerConfig towerConfig)
        {
            var towerPrefab = Resources.Load<GameObject>(towerConfig.Prefab);
            var gameObject = GameObject.Instantiate(towerPrefab, LegionEditorWindow.EditRoot.transform);
            var tower = gameObject.GetComponent<LegionTower>();
            return tower;
        }
    }
}