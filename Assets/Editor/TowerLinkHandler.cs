using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Editor
{
    public class TowerLinkHandler
    {
        public static EditTower startTarget;
        
        private static EditRoad road;
    
        public static void OnMouseDown(List<EditTower> towers)
        {
            RaycastHelper.RaycastTower(towers, out EditTower tower);
            startTarget = tower;
            if (startTarget == null)
            {
                return;
            }
            road = RoadManager.Create();
            
            UpdatePosition();
        }

        public static void OnMouseDrag()
        {
            if (startTarget == null)
            {
                return;
            }
            UpdatePosition();
        }

        private static void UpdatePosition()
        {
            var e = Event.current;
            var endPosition = RaycastHelper.RaycastYZeroPlane(e.mousePosition);
            UpdatePosition(endPosition);
        }

        private static void UpdatePosition(Vector3 endPosition)
        {
            road.gameObject.transform.position = (startTarget.gameObject.transform.position + endPosition) * 0.5f;
            road.gameObject.transform.LookAt(startTarget.gameObject.transform);
            var distance = Vector3.Distance(endPosition, startTarget.gameObject.transform.position);
            road.gameObject.transform.localScale = new Vector3(1,1,distance);
        }

        public static void OnMouseUp(List<EditTower> towers)
        {
            if (startTarget == null)
            {
                return;
            }
            RaycastHelper.RaycastTower(towers, out EditTower tower);
            if (tower == null)
            {
                GameObject.DestroyImmediate(road.gameObject);
                road = null;
            }
            else
            {
                if (startTarget.linkTowers.Contains(tower))
                {
                    OnCancel();
                    return;
                }
                
                road.Link(startTarget,tower);
            }
        }

        public static void OnCancel()
        {
            if (startTarget == null)
            {
                return;
            }
            GameObject.DestroyImmediate(road.gameObject);
            startTarget = null;
            road = null;
        }
    }
}