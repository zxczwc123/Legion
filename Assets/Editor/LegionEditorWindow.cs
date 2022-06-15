using System.Collections.Generic;
using Editor;
using UnityEditor;
using UnityEngine;

public class LegionEditorWindow : EditorWindow
{
    private ControlType m_ControlType = ControlType.Link;

    private int m_selectTowerIndex;

    private List<LegionTower> m_Towers = new List<LegionTower>();

    public static GameObject EditRoot { get; private set; }

    [MenuItem("Tools/LegionEditorWindow", false, 0)]
    private static void GetWindow()
    {
        var window = GetWindow<LegionEditorWindow>();
        window.Show();
    }

    private void OnEnable()
    {
        LegionSelection.SelectTower = null;
        InitEditRoot();
        InitTowers();
        SceneView.duringSceneGui += OnSceneGUI;
        LegionSelection.OnSelectTowerChange += OnSelectTowerChange;
    }

    private void OnDisable()
    {
        LegionSelection.SelectTower = null;
        SceneView.duringSceneGui -= OnSceneGUI;
        LegionSelection.OnSelectTowerChange -= OnSelectTowerChange;
        ReleaseEditRoot();
        ReleaseTowers();
        ConfigManager.Release();
    }

    private void InitEditRoot()
    {
        var editRootName = nameof(LegionEditorWindow);
        EditRoot = GameObject.Find(editRootName);
        if (EditRoot == null) EditRoot = new GameObject(editRootName);
    }

    private void ReleaseEditRoot()
    {
        if (EditRoot == null) return;
        DestroyImmediate(EditRoot);
        EditRoot = null;
    }

    private void InitTowers()
    {
        var levelConfig = ConfigManager.GetDict<LevelTowerConfig>();
        if (levelConfig != null)
            foreach (var towerItem in levelConfig.Values)
            {
                var tower = TowerManager.CreateTower(towerItem);
                tower.InitRoadCreator(RoadManager.Create, RoadManager.Release);
                tower.InitRoads(m_Towers);
                m_Towers.Add(tower);
            }
    }

    private void ReleaseTowers()
    {
        m_Towers.Clear();
    }

    private void AddTower()
    {
        if (TowerAddHandler.dragTarget != null)
        {
            TowerAddHandler.dragTarget.Id = m_Towers.Count;
            m_Towers.Add(TowerAddHandler.dragTarget);
        }
    }

    private void ResetId()
    {
        for (var i = 0; i < m_Towers.Count; i++)
        {
            var tower = m_Towers[i];
            tower.Id = i;
        }
    }

    private void OnSelectTowerChange()
    {
        Repaint();
    }

    private void OnGUI()
    {
        GUILayout.Label("列表");

        var emptyTips = "配置表中无数据！";
        var style = new GUIStyle("GridListText");
        var contents = GetPrefabContents();
        m_selectTowerIndex = CustomGUILayout.AspectSelectionGridImageAndText(m_selectTowerIndex, contents, 64,
            style, emptyTips);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MinHeight(10));
        if (LegionSelection.SelectTower != null)
        {
            GUILayout.Label($"当前选中ID:{LegionSelection.SelectTower.Id}");
            var count = LegionSelection.SelectTower.Count;
            LegionSelection.SelectTower.Count = EditorGUILayout.IntField("数量", count);
            var legion = LegionSelection.SelectTower.Legion;
            LegionSelection.SelectTower.Legion = EditorGUILayout.IntField("队伍", legion);
        }
        else
        {
            GUILayout.Label("无选中");
        }
        EditorGUILayout.EndVertical();
        if (GUILayout.Button("保存"))
        {
            ResetId();
            var levelConfig = new Dictionary<int, LevelTowerConfig>();
            for (var i = 0; i < m_Towers.Count; i++)
            {
                var tower = m_Towers[i];
                var config = tower.ToLevelConfig();
                levelConfig.Add(tower.Id, config);
            }
            ConfigManager.SaveDict(levelConfig);
        }
    }

    private GUIContent[] GetPrefabContents()
    {
        var towerConfig = ConfigManager.GetDict<TowerConfig>();
        var contents = new GUIContent[towerConfig.Count];
        var index = 0;
        foreach (var towerItem in towerConfig.Values)
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/" + towerItem.Prefab + ".prefab");
            var prefab = Resources.Load(towerItem.Prefab);
            Texture tex = AssetPreview.GetAssetPreview(asset);
            var content = new GUIContent();
            contents[index] = content;
            var name = prefab.name;
            content.text = name;
            content.tooltip = name;
            content.image = tex;
            index++;
        }
        return contents;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        OnDrawSceneGUI();
        var e = Event.current;
        var cid = GUIUtility.GetControlID(FocusType.Passive);
        var towerTag = DragAndDrop.GetGenericData("TowerTag");
        switch (e.keyCode)
        {
            case KeyCode.Escape:
                if (m_ControlType == ControlType.Link)
                    TowerLinkHandler.OnCancel();
                else
                    TowerDragHandler.OnCancel();
                e.Use();
                break;
        }
        switch (e.type)
        {
            case EventType.Layout:
                HandleUtility.AddDefaultControl(cid);
                break;
            case EventType.MouseDown:
                if (m_ControlType == ControlType.Link)
                    TowerLinkHandler.OnMouseDown(m_Towers);
                else if (m_ControlType == ControlType.Move) TowerDragHandler.OnMouseDown(m_Towers);
                break;
            case EventType.MouseMove:
                Debug.Log("MouseMove Copy");
                break;
            case EventType.MouseDrag:
                Debug.Log("MouseDrag");
                if (m_ControlType == ControlType.Link)
                    TowerLinkHandler.OnMouseDrag();
                else if (m_ControlType == ControlType.Move) TowerDragHandler.OnMouseDrag();
                break;
            case EventType.DragPerform:
                Debug.Log("DragPerform Scene");
                if (towerTag != null)
                {
                    AddTower();
                    TowerAddHandler.OnDragAccept();
                    DragAndDrop.SetGenericData("TowerTag", null);
                    DragAndDrop.AcceptDrag();
                    e.Use();
                }
                break;
            case EventType.DragUpdated:
                Debug.Log("DragUpdated Scene");
                if (towerTag != null)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    TowerAddHandler.OnDragUpdated();
                }
                else
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.None;
                }
                break;
            case EventType.DragExited:
                if (towerTag != null)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    TowerAddHandler.OnCancel();
                    DragAndDrop.SetGenericData("TowerTag", null);
                    DragAndDrop.AcceptDrag();
                    e.Use();
                }
                break;
            case EventType.MouseUp:
                if (m_ControlType == ControlType.Link)
                    TowerLinkHandler.OnMouseUp(m_Towers);
                else if (m_ControlType == ControlType.Move)
                    TowerDragHandler.OnMouseUp();
                else
                    DelHandler.OnMouseUp(m_Towers);
                break;
        }
    }

    private void OnDrawSceneGUI()
    {
        Handles.BeginGUI();
        if (GUI.Toggle(new Rect(10, -10, 50, 50), m_ControlType == ControlType.Link, "连线"))
            m_ControlType = ControlType.Link;
        if (GUI.Toggle(new Rect(60, -10, 50, 50), m_ControlType == ControlType.Move, "移动"))
            m_ControlType = ControlType.Move;
        if (GUI.Toggle(new Rect(110, -10, 50, 50), m_ControlType == ControlType.Del, "删除"))
            m_ControlType = ControlType.Del;
        Handles.EndGUI();
    }
}