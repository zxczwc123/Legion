using DefaultNamespace.Data;

namespace Editor
{
    public class RoadManager
    {
        public static EditTower Create(LevelConfig towerItem)
        {
            var tower = new EditTower();
            var towerInfo = new TowerInfo();
            towerInfo.Init(towerItem);
            tower.Init(towerInfo);
            return tower;
        }

        public static EditTower Create(TowerConfig towerConfig)
        {
            var tower = new EditTower();
            var towerInfo = new TowerInfo();
            var towerItem = new LevelConfig();
            towerItem.Id = 0;
            towerItem.TowerId = towerConfig.Id;
            towerItem.Pos = new float[] {0, 0, 0};
            towerInfo.Init(towerItem);
            tower.Init(towerInfo);
            return tower;
        }
    }
}