using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.Data
{
    public class TowerInfo
    {
        public int Id;
        public int TowerId;
        public int Legion;
        public Vector3 position;
        public int Count;
        public int Type;
        public int MaxCount;

        public List<int> Links;

        public GameObject gameObject;

        public void Init(LevelTowerConfig levelTowerConfig)
        {
            Id = levelTowerConfig.Id;
            TowerId = levelTowerConfig.TowerId;
            Legion = levelTowerConfig.Legion;
            position = levelTowerConfig.Pos;
            Count = levelTowerConfig.Count;
            if (levelTowerConfig.Links != null)
            {
                Links = new List<int>(levelTowerConfig.Links);
            }
            else
            {
                Links = new List<int>();
            }
        }
    }
}