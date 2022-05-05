using UnityEditor;
using UnityEngine;

public class HandleHelper
{
    public static float GetHandleSize(Vector3 point)
    {
        var size = HandleUtility.GetHandleSize(point);
        return size * 0.2f;
    }
}