using System.Collections.Generic;
using UnityEngine;

public class TowerDragHandler
{
    public static EditTower dragTarget;

    private static Vector3 prePosition;
    
    public static void OnMouseDown(List<EditTower> towers)
    {
        RaycastHelper.RaycastTower(towers, out EditTower tower);
        dragTarget = tower;
        if (dragTarget == null)
        {
            return;
        }
        var e = Event.current;
        prePosition = RaycastHelper.RaycastYZeroPlane(e.mousePosition);
    }

    public static void OnMouseDrag()
    {
        if (dragTarget == null)
        {
            return;
        }
        var e = Event.current;
        var curPosition = RaycastHelper.RaycastYZeroPlane(e.mousePosition);
        var deltaPosition = curPosition - prePosition;
        dragTarget.gameObject.transform.position += deltaPosition;
        prePosition = curPosition;
    }

    public static void OnMouseUp()
    {
        dragTarget = null;
    }
}