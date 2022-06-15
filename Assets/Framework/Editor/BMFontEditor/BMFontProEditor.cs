using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEditorInternal;
using System.IO;

public class BMFontProEditor : EditorWindow {
    [MenuItem("Framework/BMFont Maker")]
    static public void OpenBMFontMaker() {
        EditorWindow.GetWindow<BMFontProEditor>(true, "BMFont Maker", true).Show();
    }

    static bool IsPackable(object o) {
        return o != null && (o.GetType() == typeof(Sprite) || o.GetType() == typeof(Texture2D) || (o.GetType() == typeof(DefaultAsset)));
    }

    private SerializedObject serializedObject;

    [SerializeField]
    private string m_fontName;

    private SerializedProperty m_bitMapSpritesProperty;

    private BitMapFontSpriteDataDrawer m_bitMapFontSpriteDataDrawer;
    [SerializeField]
    private BitMapFontSpriteData m_bitMapSpriteData = new BitMapFontSpriteData();

    public BMFontProEditor() {
        
    }

    private void OnEnable() {
        serializedObject = new SerializedObject(this);
        m_bitMapSpritesProperty = serializedObject.FindProperty("m_bitMapSpriteData");
        m_bitMapFontSpriteDataDrawer = new BitMapFontSpriteDataDrawer();
    }


    void OnGUI() {
        serializedObject.Update();

        m_fontName = EditorGUILayout.TextField("字体名称：", m_fontName);
        HandleDropControlableRect();
        EditorGUI.BeginChangeCheck();
        m_bitMapFontSpriteDataDrawer.OnGUI(m_bitMapSpritesProperty);
        if (EditorGUI.EndChangeCheck()) {
            serializedObject.ApplyModifiedProperties();
        }
        if (GUILayout.Button("生成字体")) {
            GenerateFont();
        }
        
    }

    private void HandleDropControlableRect() {
        var currentEvent = Event.current;
        var usedEvent = false;
        

        Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(40));

        var controlID = EditorGUIUtility.hotControl;
        switch (currentEvent.type) {
            case EventType.DragExited:
                if (GUI.enabled)
                    HandleUtility.Repaint();
                break;

            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (rect.Contains(currentEvent.mousePosition) && GUI.enabled) {
                    // Check each single object, so we can add multiple objects in a single drag.
                    var didAcceptDrag = false;
                    var references = DragAndDrop.objectReferences;
                    foreach (var obj in references) {
                        if (IsPackable(obj)) {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            if (currentEvent.type == EventType.DragPerform) {
                                var fontSpriteData = new BitMapFontSprite();
                                if(obj is Texture) {
                                    var fontTexture = obj as Texture;
                                    var fonsSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(fontTexture));
                                    fontSpriteData.sprite = fonsSprite;
                                } else {
                                    fontSpriteData.sprite = obj as Sprite;
                                }
                                fontSpriteData.fontChar = obj.name;
                                m_bitMapSpriteData.fontSprites.Add(fontSpriteData);
                                DragAndDrop.activeControlID = 0;
                            } else
                                DragAndDrop.activeControlID = controlID;
                        }
                    }
                    if (didAcceptDrag) {
                        GUI.changed = true;
                        DragAndDrop.AcceptDrag();
                        usedEvent = true;
                    }
                }
                break;
        }

        var style = EditorStyles.helpBox;
        style.alignment = TextAnchor.MiddleCenter;
        EditorGUI.LabelField(rect, "可拖动Sprite到此添加", style);

        if (usedEvent)
            currentEvent.Use();
    }

    private void GenerateFont() {
        if (string.IsNullOrEmpty(m_fontName)) {
            Debug.LogWarning("字体名称不能为空");
            return;
        }
        if(m_bitMapSpriteData.fontSprites == null || m_bitMapSpriteData.fontSprites.Count == 0) {
            Debug.LogWarning("字体图片资源未设置");
            return;
        } else {
            for (var i = 0; i < this.m_bitMapSpriteData.fontSprites.Count; i++) {
                if (m_bitMapSpriteData.fontSprites[i].sprite == null) {
                    Debug.LogWarning("字体图片资源未设置");
                    return;
                }
                if (string.IsNullOrEmpty(m_bitMapSpriteData.fontSprites[i].fontChar)) {
                    Debug.LogWarning("字体字符未设置");
                    return;
                }
            }
        }
        List<Texture2D> packTexs = new List<Texture2D>();
        for(var i = 0; i < this.m_bitMapSpriteData.fontSprites.Count; i++) {
            packTexs.Add(m_bitMapSpriteData.fontSprites[i].sprite.texture);
            var spritePath = AssetDatabase.GetAssetPath(m_bitMapSpriteData.fontSprites[i].sprite.texture);
            TextureImporter atlas_Importer = AssetImporter.GetAtPath(spritePath) as TextureImporter;
            if (!atlas_Importer.isReadable) {
                atlas_Importer.isReadable = true;
                atlas_Importer.SaveAndReimport();
            }
        }
        Texture2D atlasTex = new Texture2D(1024, 1024);
        Rect[] atlasRects = atlasTex.PackTextures(packTexs.ToArray(), 1, 4096);
        var atlasPath = AssetDatabase.GetAssetPath(m_bitMapSpriteData.fontSprites[0].sprite.texture);
        atlasPath = Path.Combine(Path.GetDirectoryName(atlasPath) ,m_fontName + ".png");
        File.WriteAllBytes(atlasPath, atlasTex.EncodeToPNG());

        AssetDatabase.Refresh();

        CreateArtistFont(atlasRects, atlasPath);
    }

    private void CreateArtistFont(Rect[] atlasRects,string texturePath) {
        string dirName = Path.GetDirectoryName(texturePath);
        string fntFileName = dirName + m_fontName + ".fnt";

        Font CustomFont = null;
        {
            var fontPath = Path.Combine(dirName, m_fontName + ".fontsettings");
            CustomFont = AssetDatabase.LoadAssetAtPath(fontPath, typeof(Font)) as Font;
            if (CustomFont == null) {
                CustomFont = new Font();
                AssetDatabase.CreateAsset(CustomFont, fontPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        TextAsset BMFontText = null;
        {
            BMFontText = AssetDatabase.LoadAssetAtPath(fntFileName, typeof(TextAsset)) as TextAsset;
        }
        Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(texturePath);


        CharacterInfo[] characterInfo = new CharacterInfo[atlasRects.Length];
        for (int i = 0; i < atlasRects.Length; i++) {
            Rect spriteInfo = atlasRects[i];
            var fontChar = m_bitMapSpriteData.fontSprites[i].fontChar;
            CharacterInfo info = new CharacterInfo();
            info.index = Char.ConvertToUtf32(fontChar, 0);
            info.uv.x = (float)spriteInfo.x ;
            info.uv.y = (float)spriteInfo.y  + spriteInfo.height;
            info.uv.width = (float)spriteInfo.width;
            info.uv.height =  -(float)spriteInfo.height ;
            info.vert.x = 0;
            info.vert.y = 0;
            info.vert.width = (float)spriteInfo.width * (float)texture.width;
            info.vert.height = (float)spriteInfo.height * (float)texture.height;
            info.advance = (int)((float)spriteInfo.width * (float)texture.width);
            characterInfo[i] = info;
        }
        CustomFont.characterInfo = characterInfo;


        Material mat = null;
        {
            Shader shader = Shader.Find("Transparent/Diffuse");
            mat = new Material(shader);
            Texture tex = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture)) as Texture;
            mat.SetTexture("_MainTex", tex);
            var fontMaterialName = Path.Combine(dirName , m_fontName + ".mat");
            AssetDatabase.CreateAsset(mat, fontMaterialName);
            AssetDatabase.SaveAssets();
        }
        CustomFont.material = mat;

        AssetDatabase.Refresh();
    }

}

[Serializable]
public class BitMapFontSpriteData {
    [SerializeField]
    public List<BitMapFontSprite> fontSprites = new List<BitMapFontSprite>();
}


[Serializable]
public class BitMapFontSprite {
    [SerializeField]
    public string fontChar;
    [SerializeField]
    public Sprite sprite;
}

/// <summary>
///   
/// </summary>
[CustomPropertyDrawer(typeof(BitMapFontSpriteData), true)]
public class BitMapFontSpriteDataDrawer : PropertyDrawer {
    private ReorderableList m_ReorderableList;

    private void Init(SerializedProperty property) {

        if (m_ReorderableList != null)
            return;
        SerializedProperty array = property.FindPropertyRelative("fontSprites");
        m_ReorderableList = new ReorderableList(property.serializedObject, array);
        m_ReorderableList.drawElementCallback = DrawOptionData;
        m_ReorderableList.drawHeaderCallback = DrawHeader;
        m_ReorderableList.elementHeight += 16;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        Init(property);

        m_ReorderableList.DoList(position);
    }

    public void OnGUI(SerializedProperty property) {
        Init(property);
        Rect rect = EditorGUILayout.GetControlRect(false,m_ReorderableList.GetHeight());
        m_ReorderableList.DoList(rect);
    }

    private void DrawHeader(Rect rect) {
        GUI.Label(rect, "BitMapFontSprites");
    }

    private void DrawOptionData(Rect rect, int index, bool isActive, bool isFocused) {
        SerializedProperty itemData = m_ReorderableList.serializedProperty.GetArrayElementAtIndex(index);
        SerializedProperty itemImage = itemData.FindPropertyRelative("sprite");
        SerializedProperty itemName = itemData.FindPropertyRelative("fontChar");

        RectOffset offset = new RectOffset(0, 0, -1, -3);
        rect = offset.Add(rect);
        rect.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.PropertyField(rect, itemImage, GUIContent.none);
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, itemName, GUIContent.none);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        Init(property);

        return m_ReorderableList.GetHeight();
    }
}