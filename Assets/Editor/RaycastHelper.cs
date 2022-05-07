using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Data;
using UnityEditor;
using UnityEngine;

public class RaycastHelper
{

    public static bool RaycastTower(List<EditTower> towers,out EditTower tower)
    {
        Ray mouseRay = GUIPointToWorldRayPrecise(Event.current.mousePosition);
        if (Physics.Raycast(mouseRay, out RaycastHit hitInfo))
        {
            var towerObj = hitInfo.collider.gameObject;
            tower = towers.Find(x => x.gameObject == towerObj);
            return true;
        }
        tower = null;
        return false;
    }
        
    public static bool RaycastAllPathLinkLine(List<EditTower> towers, out EditTower start,out EditTower end)
    {
        Ray mouseRay = GUIPointToWorldRayPrecise(Event.current.mousePosition);
        start = null;
        end = null;
        for (var i = 0; i < towers.Count; i++)
        {
            var tower = towers[i];
            for (var j = 0; j < tower.towerInfo.Links.Count; j++)
            {
                var linkPointIndex = tower.towerInfo.Links[j];
                if(linkPointIndex < i) continue;
                Debug.Log(i + " " + linkPointIndex);
                var linkPoint = towers[linkPointIndex];
                    
                var midPoint = (tower.towerInfo.position + linkPoint.towerInfo.position) * 0.5f;
                var distance = MathTool.DisOfLine(tower.towerInfo.position, linkPoint.towerInfo.position - tower.towerInfo.position, mouseRay.origin,
                    mouseRay.direction );
                Debug.Log(distance);
                    
                var hitSize = HandleHelper.GetHandleSize(midPoint);
                if (distance < hitSize)
                {
                    start = tower;
                    end = linkPoint;
                    return true;
                }
            }
        }
        return false;
    }

    public static Vector3 RaycastYZeroPlane(Vector2 mousePosition)
    {
        var ray = GUIPointToWorldRayPrecise(mousePosition);
        return MathTool.IntersectLineAndPlane(ray.origin, ray.direction, Vector3.up, Vector3.zero);
    }

    public static Ray GUIPointToWorldRayPrecise(Vector2 guiPoint, float startZ = float.NegativeInfinity)
    {
        Camera camera = Camera.current;
        if (!camera)
        {
            Debug.LogError("Unable to convert GUI point to world ray if a camera has not been set up!");
            return new Ray(Vector3.zero, Vector3.forward);
        }

        if (float.IsNegativeInfinity(startZ))
        {
            startZ = camera.nearClipPlane;
        }

        Vector2 screenPixelPos = HandleUtility.GUIPointToScreenPixelCoordinate(guiPoint);
        Rect viewport = camera.pixelRect;

        Matrix4x4 camToWorld = camera.cameraToWorldMatrix;
        Matrix4x4 camToClip = camera.projectionMatrix;
        Matrix4x4 clipToCam = camToClip.inverse;

        // calculate ray origin and direction in world space
        Vector3 rayOriginWorldSpace;
        Vector3 rayDirectionWorldSpace;

        // first construct an arbitrary point that is on the ray through this screen pixel (remap screen pixel point to clip space [-1, 1])
        Vector3 rayPointClipSpace = new Vector3(
            (screenPixelPos.x - viewport.x) * 2.0f / viewport.width - 1.0f,
            (screenPixelPos.y - viewport.y) * 2.0f / viewport.height - 1.0f,
            0.95f
        );

        // and convert that point to camera space
        Vector3 rayPointCameraSpace = clipToCam.MultiplyPoint(rayPointClipSpace);

        if (camera.orthographic)
        {
            // ray direction is always 'camera forward' in orthographic mode
            Vector3 rayDirectionCameraSpace = new Vector3(0.0f, 0.0f, -1.0f);
            rayDirectionWorldSpace = camToWorld.MultiplyVector(rayDirectionCameraSpace);
            rayDirectionWorldSpace.Normalize();

            // in camera space, the ray origin has the same XY coordinates as ANY point on the ray
            // so we just need to override the Z coordinate to startZ to get the correct starting point
            // (assuming camToWorld is a pure rotation/offset, with no scale)
            Vector3 rayOriginCameraSpace = rayPointCameraSpace;
            rayOriginCameraSpace.z = startZ;

            // move it to world space
            rayOriginWorldSpace = camToWorld.MultiplyPoint(rayOriginCameraSpace);
        }
        else
        {
            // in projective mode, the ray passes through the origin in camera space
            // so the ray direction is just (ray point - origin) == (ray point)
            Vector3 rayDirectionCameraSpace = rayPointCameraSpace;
            rayDirectionCameraSpace.Normalize();

            rayDirectionWorldSpace = camToWorld.MultiplyVector(rayDirectionCameraSpace);

            // calculate the correct startZ offset from the camera by moving a distance along the ray direction
            // this assumes camToWorld is a pure rotation/offset, with no scale, so we can use rayDirection.z to calculate how far we need to move
            Vector3 cameraPositionWorldSpace = camToWorld.MultiplyPoint(Vector3.zero);
            Vector3 originOffsetWorldSpace = rayDirectionWorldSpace * Mathf.Abs(startZ / rayDirectionCameraSpace.z);
            rayOriginWorldSpace = cameraPositionWorldSpace + originOffsetWorldSpace;
        }

        return new Ray(rayOriginWorldSpace, rayDirectionWorldSpace);
    }
}