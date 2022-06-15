using UnityEngine;

namespace DefaultNamespace
{
    public class ConfigManager
    {
        public static LevelConfig GetLevelConfig(int level)
        {
            var levelConfig = Resources.Load<LevelConfig>($"Config/LevelConfig{level}");
            return levelConfig;
        }
    }
}