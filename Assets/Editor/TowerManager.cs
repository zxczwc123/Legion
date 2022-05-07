using DefaultNamespace.Data;
using UnityEngine;

namespace Editor
{
    public class TowerManager
    {
        public static EditTower CreateTower(LevelConfig towerItem)
        {
            var tower = new EditTower();
            var towerInfo = new TowerInfo();
            towerInfo.Init(towerItem);
            tower.Init(towerInfo);
            return tower;
        }

        public static EditTower CreateTower(TowerConfig towerConfig)
        {
            var tower = new EditTower();
            var towerInfo = new TowerInfo();
            var towerItem = new LevelConfig();
            towerItem.Id = 0;
            towerItem.TowerId = towerConfig.Id;
            towerItem.Pos = Vector3.zero;
            towerInfo.Init(towerItem);
            tower.Init(towerInfo);
            return tower;
        }
    }
}