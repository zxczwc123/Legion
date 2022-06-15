using System.Collections.Generic;
using UnityEngine;

public class TowerDragHandler
{
    public static LegionTower dragTarget;

    private static Vector3 prePosition;

    private static Vector3 startPosition;

    public static void OnMouseDown(List<LegionTower> towers)
    {
        RaycastHelper.RaycastTower(towers, out var tower);
        dragTarget = tower;
        if (dragTarget == null)
        {
            LegionSelection.SelectTower = null;
            return;
        }
        LegionSelection.SelectTower = tower;
        var e = Event.current;
        startPosition = RaycastHelper.RaycastYZeroPlane(e.mousePosition);
        prePosition = startPosition;
    }

    public static void OnMouseDrag()
    {
        if (dragTarget == null) return;
        var e = Event.current;
        var curPosition = RaycastHelper.RaycastYZeroPlane(e.mousePosition);
        var deltaPosition = curPosition - prePosition;
        dragTarget.gameObject.transform.position += deltaPosition;
        prePosition = curPosition;

        dragTarget.UpdateRoad();
    }

    public static void OnMouseUp()
    {
        dragTarget = null;
    }

    public static void OnCancel()
    {
        if (dragTarget != null)
        {
            dragTarget.gameObject.transform.position = startPosition;
            dragTarget.UpdateRoad();
            dragTarget = null;
        }
    }
}