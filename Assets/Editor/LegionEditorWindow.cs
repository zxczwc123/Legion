using System.Collections.Generic;
using DefaultNamespace.Data;
using Editor;
using UnityEditor;
using UnityEngine;

public class LegionEditorWindow : EditorWindow
{
    [MenuItem("Tools/LegionEditorWindow", false, 0)]
    private static void GetWindow()
    {
        var window = EditorWindow.GetWindow<LegionEditorWindow>();
        window.Show();
    }

    public static GameObject EditRoot { get; private set; }

    private List<EditTower> m_Towers = new List<EditTower>();

    private ControlType m_ControlType = ControlType.Link;

    private int m_selectTowerIndex;

    private void OnEnable()
    {
        InitEditRoot();
        SceneView.duringSceneGui += OnSceneGUI;
        var levelConfig = ConfigManager.GetDict<LevelConfig>();
        foreach (var towerItem in levelConfig.Values)
        {
            var tower = new EditTower();
            var towerInfo = new TowerInfo();
            towerInfo.Init(towerItem);
            tower.Init(towerInfo);
            m_Towers.Add(tower);
        }
    }

    private void OnDisable()
    {
        ReleaseEditRoot();
        SceneView.duringSceneGui += OnSceneGUI;
        ConfigManager.Release();
    }

    private void InitEditRoot()
    {
        var editRootName = nameof(LegionEditorWindow);
        EditRoot = GameObject.Find(editRootName);
        if (EditRoot == null)
        {
            EditRoot = new GameObject(editRootName);
        }
    }

    private void ReleaseEditRoot()
    {
        if (EditRoot == null)
        {
            return;
        }
        DestroyImmediate(EditRoot);
        EditRoot = null;
    }

    private void OnGUI()
    {
        var emptyTips = "配置表中无数据！";
        var style = new GUIStyle("GridListText");
        var contents = GetPrefabContents();
        m_selectTowerIndex = CustomGUILayout.AspectSelectionGridImageAndText(m_selectTowerIndex, contents, 64,
            style, emptyTips, out bool doubleClick);
    }

    private GUIContent[] GetPrefabContents()
    {
        var towerConfig = ConfigManager.GetDict<TowerConfig>();
        var contents = new GUIContent[towerConfig.Count];
        var index = 0;
        foreach (var towerItem in towerConfig)
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/" + towerItem.Value.Prefab + ".prefab");
            var prefab = Resources.Load(towerItem.Value.Prefab);
            Texture tex = AssetPreview.GetAssetPreview(asset);
            var content = new GUIContent();
            contents[index] = content;
            var name = prefab.name;
            content.text = name;
            content.tooltip = name;
            index++;
        }
        return contents;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        OnDrawSceneGUI();
        var e = Event.current;
        switch (e.type)
        {
            case EventType.Layout:
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                break;
            case EventType.MouseDown:
                TowerDragHandler.OnMouseDown(m_Towers);
                break;
            case EventType.MouseDrag:
                TowerDragHandler.OnMouseDrag();
                break;
            case EventType.MouseUp:
                TowerDragHandler.OnMouseUp();
                break;
        }
    }

    private void OnDrawSceneGUI()
    {
        Handles.BeginGUI();
        if (GUI.Toggle(new Rect(10, -10, 50, 50), m_ControlType == ControlType.Link, "连线"))
        {
            m_ControlType = ControlType.Link;
        }
        if (GUI.Toggle(new Rect(60, -10, 50, 50), m_ControlType == ControlType.Move, "移动"))
        {
            m_ControlType = ControlType.Move;
        }
        Handles.EndGUI();
    }
}