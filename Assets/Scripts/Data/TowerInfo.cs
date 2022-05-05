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

        public void Init(LevelConfig levelConfig)
        {
            Id = levelConfig.Id;
            TowerId = levelConfig.TowerId;
            Legion = levelConfig.Legion;
            if (levelConfig.Pos != null)
            {
                position = new Vector3(levelConfig.Pos[0],levelConfig.Pos[1],levelConfig.Pos[2]);
            }
            Count = levelConfig.Count;
            if (levelConfig.Links != null)
            {
                Links = new List<int>(levelConfig.Links);
            }
            else
            {
                Links = new List<int>();
            }
        }
    }
}