using System.Linq;
using Editor;
using UnityEditor;
using UnityEngine;

public class TowerAddHandler
{
    public static EditTower dragTarget;

    public static void OnDragUpdated()
    {
        if (dragTarget == null)
        {
            var index = (int)DragAndDrop.GetGenericData("index");
            var towerConfig = ConfigManager.GetDict<TowerConfig>();
            var config = towerConfig.ToList()[index].Value;
            dragTarget = TowerManager.CreateTower(config);
        }
        var e = Event.current;
        var mousePosition = RaycastHelper.RaycastYZeroPlane(e.mousePosition);
        dragTarget.gameObject.transform.position = mousePosition;
    }

    public static void OnDragAccept()
    {
        dragTarget = null;
    }

    public static void OnCancel()
    {
        if (dragTarget != null)
        {
            GameObject.DestroyImmediate(dragTarget.gameObject);
            dragTarget = null;
        }
    }
}