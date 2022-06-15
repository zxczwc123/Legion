using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using LitJson;
using System;
using Framework.MEditor.Utility;
using System.Xml.Serialization;
using System.Xml;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;

namespace Framework.MEditor
{
    public class TextureEditorMenu{
        
        [MenuItem("Assets/资源工具/自动切图", false, 0)]
        public static void TrimTexture()
        {
            TextureEditor.TrimTexture();
        }

        [MenuItem("Assets/资源工具/分割图片", false, 0)]
        public static void SplitTexture()
        {
            TextureEditor.SplitTexture();
        }

        [MenuItem("Assets/资源工具/自动切图", true, 0)]
        [MenuItem("Assets/资源工具/分割图片", true, 0)]
        public static bool CheckTrimTexture()
        {
            if(Selection.activeObject == null)
            {
                return false;
            }
            string packAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (".json".Equals(packAssetPath.Extension()))
                return true;
            if (".txt".Equals(packAssetPath.Extension()))
                return true;
            if (".plist".Equals(packAssetPath.Extension()))
                return true;
            return false;
        }
    }

    public class TextureEditor : MonoBehaviour
    {
        

        public static void TrimTexture()
        {
            if (Selection.activeObject == null)
            {
                EditorUtility.DisplayDialog("提示", "请右键选择贴图对应的json、plist、txt文件执行操作。", "确定");
                return;
            }
            SpriteData spriteData = null;
            string spriteDataPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (Selection.activeObject is TextAsset)
            {
                TextAsset spriteTextAsset = Selection.activeObject as TextAsset;
                if (".json".Equals(spriteDataPath.Extension()))
                {
                    TextAsset textAsset = Selection.activeObject as TextAsset;
                    spriteData = new SpriteJsonData(textAsset.text);
                }
                else if (".txt".Equals(spriteDataPath.Extension()))
                {
                    TextAsset textAsset = Selection.activeObject as TextAsset;
                    spriteData = new SpriteJsonData(textAsset.text);
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "请右键选择贴图对应的json、plist、txt文件执行操作。", "确定");
                    return;
                }
            }
            else if (Selection.activeObject is DefaultAsset)
            {
                if (".plist".Equals(spriteDataPath.Extension()))
                {
                    string text = TxtUtility.FileToString(spriteDataPath);
                    spriteData = new SpriteXmlData(text);
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "请右键选择贴图对应的json、plist、txt文件执行操作。", "确定");
                    return;
                }
            }
            string texturePath = spriteDataPath.Replace(spriteDataPath.Extension(), spriteData.imageExtention);
            AssetImporter jsonImporter = AssetImporter.GetAtPath(spriteDataPath);
            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(texturePath);
            List<SpriteMetaData> oldSheet = new List<SpriteMetaData>(textureImporter.spritesheet);
            SpriteMetaData[] newSheet = spriteData.spritesheet;
            int count = spriteData.count;
            string info = "";
            float progress = 0;
            for (int i = 0; i < count; i++)
            {
                SpriteMetaData olds = oldSheet.Find(x => x.name == newSheet[i].name);
                if (!string.IsNullOrEmpty(olds.name)) newSheet[i].border = olds.border;
                info = "切图中" + (int)((float)i / count * 100) + "%";
                progress = (float)i / count;
                EditorUtility.DisplayProgressBar("正在切图", info, progress);
            }
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.spritePackingTag = "tag";
            textureImporter.spritesheet = newSheet;
            textureImporter.sRGBTexture = false;
            textureImporter.filterMode = FilterMode.Bilinear;
            textureImporter.maxTextureSize = 1024;
            textureImporter.textureCompression = TextureImporterCompression.Compressed;

            EditorUtility.ClearProgressBar();
            AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
        }

        public static void SplitTexture()
        {
            if (Selection.activeObject == null)
            {
                EditorUtility.DisplayDialog("提示", "请右键选择贴图对应的json、plist、txt文件执行操作。", "确定");
                return;
            }
            SpriteData spriteData = null;
            string spriteDataPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (Selection.activeObject is TextAsset)
            {
                TextAsset spriteTextAsset = Selection.activeObject as TextAsset;
                if (".json".Equals(spriteDataPath.Extension()))
                {
                    TextAsset textAsset = Selection.activeObject as TextAsset;
                    spriteData = new SpriteJsonData(textAsset.text);
                }
                else if (".txt".Equals(spriteDataPath.Extension()))
                {
                    TextAsset textAsset = Selection.activeObject as TextAsset;
                    spriteData = new SpriteJsonData(textAsset.text);
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "请右键选择贴图对应的json、plist、txt文件执行操作。", "确定");
                    return;
                }
            }
            else if (Selection.activeObject is DefaultAsset)
            {
                if (".plist".Equals(spriteDataPath.Extension()))
                {
                    string text = TxtUtility.FileToString(spriteDataPath);
                    spriteData = new SpriteXmlData(text);
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "请右键选择贴图对应的json、plist、txt文件执行操作。", "确定");
                    return;
                }
            }
            string texturePath = spriteDataPath.Replace(spriteDataPath.Extension(), spriteData.imageExtention);
            Texture2D texture = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
            SpriteMetaData[] newSheet = spriteData.spritesheet;
            string path = EditorUtility.SaveFolderPanel("选择路径","","");
            foreach (SpriteMetaData meta in newSheet)
            {
                int w = (int)meta.rect.width;
                int h = (int)meta.rect.height;
                Texture2D tex = new Texture2D(w, h);
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        tex.SetPixel(x, y, texture.GetPixel((int)meta.rect.x + x, (int)meta.rect.y + y));
                    }
                }
                string outPath = Path.Combine(path, meta.name + ".png");
                using (FileStream fs = new FileStream(outPath, FileMode.Create))
                {
                    byte[] data = tex.EncodeToPNG();
                    fs.Write(data, 0, data.Length);
                }
            }
        }

        private abstract class SpriteData
        {
            public abstract SpriteMetaData[] spritesheet
            {
                get;
            }

            public abstract int count
            {
                get;
            }

            public abstract string imageExtention
            {
                get;
            }
        }

        private class SpriteXmlData : SpriteData
        {
            private Spritesheet m_spritesheet;

            public override int count
            {
                get
                {
                    return this.m_spritesheet.Sprites.Count;
                }
            }

            public override string imageExtention
            {
                get
                {
                    return this.m_spritesheet.Name.Extension();
                }
            }

            public override SpriteMetaData[] spritesheet
            {
                get
                {
                    return new List<SpriteMetaData>(this.m_spritesheet.Sprites.Values).ToArray();
                }
            }

            public SpriteXmlData(string data)
            {
                this.m_spritesheet = Spritesheet.LoadSpriteSheet(data);
            }
        }

        private class SpriteJsonData : SpriteData
        {
            private TextureJsonStruct m_jsonStruct;

            private SpriteMetaData[] m_spritesheet;
            public override SpriteMetaData[] spritesheet
            {
                get
                {
                    if (this.m_spritesheet == null)
                    {
                        List<SpriteMetaData> metas = new List<SpriteMetaData>();
                        foreach (KeyValuePair<string, JsonFrame> pair in this.m_jsonStruct.frames)
                        {
                            SpriteMetaData meta = new SpriteMetaData();
                            meta.alignment = (int)SpriteAlignment.Center;
                            meta.border = Vector4.zero;
                            meta.name = pair.Key.NameWithoutExtension();
                            meta.pivot = Vector2.one * 0.5f;
                            meta.rect = ToReal(pair.Value.frame.rect);
                            metas.Add(meta);
                        }
                        this.m_spritesheet = metas.ToArray();
                    }
                    return this.m_spritesheet;
                }
            }

            public override int count
            {
                get { return spritesheet.Length; }
            }

            public override string imageExtention
            {
                get
                {
                    string image = this.m_jsonStruct.meta.image;
                    return image.Extension();
                }
            }

            private SpriteJsonData() { }

            public SpriteJsonData(string jsonData)
            {
                this.m_jsonStruct = JsonMapper.ToObject<TextureJsonStruct>(new JsonReader(jsonData));
            }

            private Rect ToReal(Rect rect)
            {
                Vector2 size = this.m_jsonStruct.meta.size.vector2;
                return new Rect(rect.x, size.y - rect.y - rect.height, rect.width, rect.height);
            }

            [System.Serializable]
            public class TextureJsonStruct
            {
                public Dictionary<string, JsonFrame> frames;
                public JsonMeta meta;
            }

            [System.Serializable]
            public class JsonFrame
            {
                public JsonRect frame;
                public bool rotated;
                public bool trimmed;
                public JsonRect spriteSourceSize;
                public JsonVector2 sourceSize;
            }

            [System.Serializable]
            public class JsonMeta
            {
                public string app;
                public string version;
                public string image;
                public string format;
                public JsonVector2 size;
                public string scale;
                public string smartupdate;
            }

            [System.Serializable]
            public struct JsonRect
            {
                public Rect rect
                {
                    get { return new Rect(x, y, w, h); }
                }
                public float x;
                public float y;
                public float w;
                public float h;
            }

            [System.Serializable]
            public struct JsonVector2
            {
                public Vector2 vector2
                {
                    get { return new Vector2(w, h); }
                }
                public float w;
                public float h;
            }
        }
    }

    public class PlistSerializer
    {

        public static T Deserialize<T>(string xml)
        {
            Type type = typeof(T);
            FieldInfo[] fields = type.GetFields();
            List<SerializeField> serializeFields = new List<SerializeField>();
            T result = (T)System.Activator.CreateInstance(type);
            foreach (FieldInfo field in fields)
            {
                Type fieldType = field.GetType();
                if (fieldType.IsSerializable)
                {
                    SerializeField serializeField = GetField(field);
                    serializeFields.Add(serializeField);
                }
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNodeList node = xmlDoc.SelectSingleNode("plist").ChildNodes;
            foreach (XmlElement rootChild in node)
            {
                foreach (XmlElement xe in rootChild)
                {

                }
            }
            return result;
        }

        public static SerializeField GetField(FieldInfo field)
        {
            SerializeField result = new SerializeField();
            Type fieldType = field.GetType();
            result.fieldInfo = field;
            if (typeof(System.Collections.IDictionary).IsAssignableFrom(fieldType))
            {

            }
            return null;
        }
    }

    public class SerializeField
    {
        public bool isDict;
        public FieldInfo fieldInfo;
        public Type dictType;
        public List<SerializeField> serializeFields;
    }

    class Spritesheet
    {
        public string Name { get; set; }

        public Dictionary<string, SpriteMetaData> Sprites { get; set; }

        public Spritesheet(string name)
        {
            this.Sprites = new Dictionary<string, SpriteMetaData>();
            this.Name = name;
        }

        public static Spritesheet LoadSpriteSheet(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode metadata = doc.SelectSingleNode("/plist/dict/key[.='metadata']");
            XmlNode realTextureFileName =
            metadata.NextSibling.SelectSingleNode("key[.='realTextureFileName']");
            string spritesheetName = realTextureFileName.NextSibling.InnerText;
            XmlNode textureSizeNode =
            metadata.NextSibling.SelectSingleNode("key[.='size']");
            Vector2 textureSize = parsePoint(textureSizeNode.NextSibling.InnerText);
            XmlNode frames = doc.SelectSingleNode("/plist/dict/key[.='frames']");
            XmlNodeList list = frames.NextSibling.SelectNodes("key");

            Spritesheet spritesheet = new Spritesheet(spritesheetName);

            foreach (XmlNode node in list)
            {
                string spriteName = node.InnerText;
                XmlNode dict = node.NextSibling;
                if (dict.SelectSingleNode("key[.='frame']") != null)
                {
                    string strRectangle = dict.SelectSingleNode
                    ("key[.='frame']").NextSibling.InnerText;
                    string strOffset = dict.SelectSingleNode
                    ("key[.='offset']").NextSibling.InnerText;
                    string strSourceRect = dict.SelectSingleNode
                    ("key[.='sourceColorRect']").NextSibling.InnerText;
                    string strSourceSize = dict.SelectSingleNode
                    ("key[.='sourceSize']").NextSibling.InnerText;
                    Rect frame = parseRectangle(strRectangle);
                    Vector2 offset = parsePoint(strOffset);
                    Rect sourceRectangle = parseRectangle(strSourceRect);
                    Vector2 size = parsePoint(strSourceSize);

                    string spriteFrameName = node.InnerText;
                    SpriteMetaData sprite = new SpriteMetaData();
                    sprite.alignment = (int)SpriteAlignment.Center;
                    sprite.border = Vector4.zero;
                    sprite.name = spriteName.NameWithoutExtension();
                    sprite.pivot = Vector2.one * 0.5f;
                    sprite.rect = new Rect(frame.x, (textureSize.y - frame.y - frame.height), frame.width, frame.height);
                    spritesheet.Sprites.Add(spriteFrameName, sprite);
                }
                else
                {
                    string strWidth = dict.SelectSingleNode
                    ("key[.='width']").NextSibling.InnerText;
                    string strHeight = dict.SelectSingleNode
                    ("key[.='height']").NextSibling.InnerText;
                    string strX = dict.SelectSingleNode
                    ("key[.='x']").NextSibling.InnerText;
                    string strY = dict.SelectSingleNode
                    ("key[.='y']").NextSibling.InnerText;
                    int x = int.Parse(strX);
                    int y = int.Parse(strY);
                    int w = int.Parse(strWidth);
                    int h = int.Parse(strHeight);
                    Rect frame = new Rect(x, y, w, h);

                    string spriteFrameName = node.InnerText;
                    SpriteMetaData sprite = new SpriteMetaData();
                    sprite.alignment = (int)SpriteAlignment.Center;
                    sprite.border = Vector4.zero;
                    sprite.name = spriteName.NameWithoutExtension();
                    sprite.pivot = Vector2.one * 0.5f;
                    sprite.rect = new Rect(frame.x, (textureSize.y - frame.y - frame.height), frame.width, frame.height);
                    spritesheet.Sprites.Add(spriteFrameName, sprite);
                }
            }
            return spritesheet;
        }

        private static Rect parseRectangle(string rectangle)
        {
            Regex expression = new Regex(@"\{\{(\d+),(\d+)\},\{(\d+),(\d+)\}\}");
            Match match = expression.Match(rectangle);
            if (match.Success)
            {
                int x = int.Parse(match.Groups[1].Value);
                int y = int.Parse(match.Groups[2].Value);
                int w = int.Parse(match.Groups[3].Value);
                int h = int.Parse(match.Groups[4].Value);
                return new Rect(x, y, w, h);
            }
            return Rect.zero;
        }

        private static Vector2 parsePoint(string point)
        {
            Regex expression = new Regex(@"\{(\d+),(\d+)\}");
            Match match = expression.Match(point);
            if (match.Success)
            {
                int x = int.Parse(match.Groups[1].Value);
                int y = int.Parse(match.Groups[2].Value);
                return new Vector2(x, y);
            }
            return Vector2.zero;
        }
    }
}

