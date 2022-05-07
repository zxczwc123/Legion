using System.Collections.Generic;
using UnityEngine;

namespace Editor
{
    public class DelHandler
    {
        public static void OnMouseUp(List<EditTower> towers)
        {
            RaycastHelper.RaycastTower(towers, out EditTower tower);
            if (tower == null)
            {
                
            }
            else
            {
                tower.RemoveAllRoads();
                towers.Remove(tower);
                GameObject.DestroyImmediate(tower.gameObject);
            }
        }
    }
}