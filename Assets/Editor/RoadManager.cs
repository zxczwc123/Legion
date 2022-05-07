using UnityEngine;

namespace Editor
{
    public class RoadManager
    {
        public static EditRoad Create()
        {
            var roadObjPrefab = Resources.Load<GameObject>("Prefabs/Road");
            var roadObj = GameObject.Instantiate(roadObjPrefab);
            var road = new EditRoad();
            road.gameObject = roadObj;
            road.gameObject.transform.SetParent(LegionEditorWindow.EditRoot.transform);
            return road;
        }
    }
}