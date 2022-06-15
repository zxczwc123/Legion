using UnityEngine;

namespace Editor
{
    public class RoadManager
    {
        public static LegionRoad Create()
        {
            var roadObjPrefab = Resources.Load<GameObject>("Prefabs/Road");
            var roadObj = GameObject.Instantiate(roadObjPrefab);
            var road = roadObj.GetComponent<LegionRoad>();
            road.transform.SetParent(LegionEditorWindow.EditRoot.transform);
            return road;
        }

        public static void Release(LegionRoad road)
        {
            GameObject.DestroyImmediate(road.gameObject);
        }
    }
}