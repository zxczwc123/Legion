﻿using System;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class CustomGUILayout
    {
        public static int AspectSelectionGridImageAndText(int selected, GUIContent[] textures, int approxSize,
            GUIStyle style, string emptyString)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MinHeight(10));
            var selectIndex = 0;

            if (textures.Length != 0)
            {
                var rect = GetBrushAspectRect(textures.Length, approxSize, 12, out var xCount);

                var evt = Event.current;
                var cid = EditorGUIUtility.GetControlID(FocusType.Passive);
                var type = evt.GetTypeForControl(cid);
                switch (type)
                {
                    case EventType.MouseDown:
                        if (evt.clickCount == 2 && rect.Contains(evt.mousePosition)) evt.Use();
                        break;
                    case EventType.MouseUp:
                        DragAndDrop.SetGenericData("TowerTag", null);
                        break;
                    case EventType.MouseDrag:

                        if (rect.Contains(evt.mousePosition))
                        {
                            var index = GetDragRectIndex(evt.mousePosition, rect, textures.Length, xCount);
                            if (index >= 0 && index < textures.Length)
                            {
                                Debug.Log($"MouseDrag ========================{index}");
                                DragAndDrop.PrepareStartDrag();
                                DragAndDrop.SetGenericData("index", index);
                                DragAndDrop.SetGenericData("TowerTag", "TowerTag");
                                DragAndDrop.StartDrag("tower");
                                DragAndDrop.activeControlID = cid;
                            }
                        }
                        break;
                    case EventType.DragPerform:
                        break;
                    case EventType.DragUpdated:
                        Debug.Log("DragUpdated");
                        break;
                    case EventType.DragExited:
                        Debug.Log("DragExited ========================");
                        GUIUtility.hotControl = 0;
                        break;
                }

                selectIndex = GUI.SelectionGrid(rect, Math.Min(selected, textures.Length - 1), textures, xCount,
                    style);
            }
            else
            {
                GUILayout.Label(emptyString);
            }

            GUILayout.EndVertical();
            return selectIndex;
        }

        private static Rect GetBrushAspectRect(int elementCount, int approxSize, int extraLineHeight, out int xCount)
        {
            xCount = (int) Mathf.Ceil((EditorGUIUtility.currentViewWidth - 20) / approxSize);
            var yCount = elementCount / xCount;
            if (elementCount % xCount != 0)
                yCount++;
            var r1 = GUILayoutUtility.GetAspectRect(xCount / (float) yCount);
            var r2 = GUILayoutUtility.GetRect(10, extraLineHeight * yCount);
            r1.height += r2.height;
            return r1;
        }

        private static int GetDragRectIndex(Vector3 mousePosition, Rect rect, int elementCount, int xCount)
        {
            var itemWidth = rect.width / xCount;
            var itemHeight = rect.height / Mathf.Ceil(elementCount / (float) xCount);
            for (var i = 0; i < elementCount; i++)
            {
                var col = i % xCount;
                var row = i / xCount;
                var itemRect = new Rect(rect);
                itemRect.x += col * itemWidth;
                itemRect.y += row * itemHeight;
                itemRect.width = itemWidth;
                itemRect.height = itemHeight;
                if (itemRect.Contains(mousePosition)) return i;
            }
            return -1;
        }
    }
}