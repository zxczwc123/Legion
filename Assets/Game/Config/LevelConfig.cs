using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create LevelConfig", fileName = "LevelConfig", order = 0)]
public class LevelConfig : ScriptableObject
{
    public List<LevelTowerConfig> towerConfigs;
}