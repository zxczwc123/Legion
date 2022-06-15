// ========================================================
// 描 述：VersionEditor.cs 
// 作 者： 
// 时 间：2020/07/22 21:29:41 
// 版 本：2019.2.1f1 
// ========================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Framework.Core;
using Framework.MEditor.AssetBundleEditor;
using Framework.Net;
using Framework.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework.MEditor {


    [Serializable]
    public class BuildToolEditor : EditorWindow {
        [MenuItem("Framework/BuildTool", false, 1)]
        public static void ShowWindow() {
            BuildToolEditor window = EditorWindow.GetWindow<BuildToolEditor>(true, "BuildTool");
        }

        private SerializedObject serializedObject;

        private string m_packageVersion = "1.0.0";

        private string m_packageUrl = "Http://localhost/";

        /// <summary>
        /// 代码编译标志
        /// </summary>
        private string BUILD_RELEASE = "BUILD_RELEASE";

        /// <summary>
        /// 版本信息生成标志
        /// </summary>
        private string KEY_RELEASE = "KEY_RELEASE";

        /// <summary>
        /// 
        /// </summary>
        private string KEY_RELEASE_URL = "KEY_RELEASE_ZWDZZ_URL";
        private string KEY_DEBUG_URL = "KEY_DEBUG_ZWDZZ_URL";

        private bool _isBuildReleaseSymbolDefine = false;

        private bool _isRelase = false;

        private SerializedProperty m_baseAssetPathProperty;
        private ListObjectDrawer m_baseAssetPathDrawer;
        [UnityEngine.SerializeField]
        public List<UnityEngine.Object> m_baseAssetPath = new List<UnityEngine.Object>();

        private void OnEnable() {
            serializedObject = new SerializedObject(this);
            m_baseAssetPathProperty = serializedObject.FindProperty("m_baseAssetPath");
            m_baseAssetPathDrawer = new ListObjectDrawer();


            var oldVersion = GetOldVerion();
            if (oldVersion != null) {
                if (!string.IsNullOrEmpty(oldVersion.packageUrl)) {
                    m_packageVersion = oldVersion.version;
                }
                m_packageUrl = oldVersion.packageUrl;
            }
            _isRelase = EditorPrefs.GetBool(KEY_RELEASE);
            _isBuildReleaseSymbolDefine = GetScriptingDefineSymbol(BUILD_RELEASE);

            if (_isRelase) {
                var packageUrl = EditorPrefs.GetString(KEY_RELEASE_URL);
                if (!string.IsNullOrEmpty(packageUrl)) {
                    m_packageUrl = packageUrl;
                }
            } else {
                var packageUrl = EditorPrefs.GetString(KEY_DEBUG_URL);
                if (!string.IsNullOrEmpty(packageUrl)) {
                    m_packageUrl = packageUrl;
                }
            }
        }

        public static void SetScriptingDefineSymbol(string symbol, bool define) {
            BuildTargetGroup targetGroup;
#if UNITY_ANDROID
            targetGroup = BuildTargetGroup.Android; //所有宏定义 ; 分割
#elif UNITY_IOS
            targetGroup = BuildTargetGroup.iOS; //所有宏定义 ; 分割
#else
            targetGroup = BuildTargetGroup.Standalone;
#endif
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            List<string> defineSymbols = new List<string>(symbols.Split(';'));
            if (define) {
                if (!defineSymbols.Contains(symbol)) {
                    defineSymbols.Add(symbol);
                }
            } else {
                if (defineSymbols.Contains(symbol)) {
                    defineSymbols.Remove(symbol);
                }
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", defineSymbols.ToArray()));
        }

        public static bool GetScriptingDefineSymbol(string symbol) {
            BuildTargetGroup targetGroup;
#if UNITY_ANDROID
            targetGroup = BuildTargetGroup.Android; //所有宏定义 ; 分割
#elif UNITY_IOS
            targetGroup = BuildTargetGroup.iOS; //所有宏定义 ; 分割
#else
            targetGroup = BuildTargetGroup.Standalone;
#endif
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            List<string> defineSymbols = new List<string>(symbols.Split(';'));
            return defineSymbols.Contains(symbol);
        }


        void OnGUI() {
            serializedObject.Update();
#if UNITY_ANDROID
            EditorGUILayout.LabelField(@"当前打包平台：ANDROID");
#elif UNITY_IOS
            EditorGUILayout.LabelField(@"当前打包平台：IOS");
#endif

            BeginSettingsBox(new GUIContent("【GenerateAssetBundle】"));
            {
                EditorGUILayout.LabelField(@"说明：资源产生变化必须重新生成，资源目录下不能同时存在文件夹与资源");
                if (GUILayout.Button("GenerateAssetBundle", GUILayout.Height(17))) {
                    AssetBundlePacker.GenerateAssetBundle();
                }

                EditorGUILayout.LabelField(@"说明：以下四个功能用于打安装包时非打包平台的资源移除和恢复处理。");
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("CutAndroidAssetBundleToTemp", GUILayout.Height(17))) {
                        CutAndroidAssetBundleToTemp();
                    }
                    if (GUILayout.Button("CopyAndroidAssetBundleFormTemp", GUILayout.Height(17))) {
                        CopyAndroidAssetBundleFormTemp();
                    }
                }
                
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("CutIosAssetBundleToTemp", GUILayout.Height(17))) {
                        CutIosAssetBundleToTemp();
                    }
                    if (GUILayout.Button("CopyIosAssetBundleFormTemp", GUILayout.Height(17))) {
                        CopyIosAssetBundleFormTemp();
                    }
                }

                EditorGUILayout.EndHorizontal();

                
                
                // 此功能未完成
                //EditorGUI.BeginChangeCheck();
                //m_baseAssetPathDrawer.OnGUI(m_baseAssetPathProperty);
                //if (EditorGUI.EndChangeCheck()) {
                //    serializedObject.ApplyModifiedProperties();
                //}
                //if (GUILayout.Button("RemoveAssetBundleExceptBase", GUILayout.Height(17))) {

                //}
            }
            EndSettingsBox();

            BeginSettingsBox(new GUIContent("【GenerateDLL】"));
            {
                EditorGUILayout.LabelField(@"说明：
第一种直接工具生成，
第二种采用Assembly Define自动生成，复制到Streaming目录,
BUILD_RELEASE 宏用来区分正式包和测试包,代码中无使用，则可忽略", GUILayout.MinHeight(58));
                EditorGUI.BeginChangeCheck();
                _isBuildReleaseSymbolDefine = EditorGUILayout.Toggle("BUILD_RELEASE", _isBuildReleaseSymbolDefine);
                if (EditorGUI.EndChangeCheck()) {
                    SetScriptingDefineSymbol(BUILD_RELEASE, _isBuildReleaseSymbolDefine);
                }
                if (GUILayout.Button("GenerateDLL", GUILayout.Height(17))) {
                    DllCompiler.DllCompiler.GenerateDLL();
                }
                if (GUILayout.Button("CopyDLLFrom", GUILayout.Height(17))) {
                    var gameDllLibraryPath = Path.Combine(Environment.CurrentDirectory, "Library/ScriptAssemblies", PathUtility.GetHotFixDllFileName());
                    var gameDllStreamingPath = PathUtility.GetStreamingHotFixDllPath();
                    if (File.Exists(gameDllLibraryPath)) {
                        File.Copy(gameDllLibraryPath, gameDllStreamingPath, true);
                        Debug.Log("复制成功到Streaming目录");
                    } else {
                        Debug.LogWarning(string.Format("热更文件{0}不存在,请确保是使用Assembly Definition。", gameDllLibraryPath));
                    }
                }
            }
            EndSettingsBox();

            BeginSettingsBox(new GUIContent("【GenerateVersion】"));
            {
                EditorGUILayout.LabelField(@"说明：资源产生变化或者代码产生变化必须重新生成");
                EditorGUI.BeginChangeCheck();
                _isRelase = EditorGUILayout.Toggle("RELEASE_VERSION", _isRelase);
                if (EditorGUI.EndChangeCheck()) {
                    EditorPrefs.SetBool(KEY_RELEASE, _isRelase);
                    if (_isRelase) {
                        var packageUrl = EditorPrefs.GetString(KEY_RELEASE_URL);
                        if (!string.IsNullOrEmpty(packageUrl)) {
                            m_packageUrl = packageUrl;
                        }
                    } else {
                        var packageUrl = EditorPrefs.GetString(KEY_DEBUG_URL);
                        if (!string.IsNullOrEmpty(packageUrl)) {
                            m_packageUrl = packageUrl;
                        }
                    }
                }
                EditorGUILayout.LabelField("packageVersion:");
                m_packageVersion = EditorGUILayout.TextField(m_packageVersion);
                EditorGUILayout.LabelField("packageUrl:");
                EditorGUI.BeginChangeCheck();
                m_packageUrl = EditorGUILayout.TextField(m_packageUrl);
                if (EditorGUI.EndChangeCheck()) {
                    if (this._isRelase) {
                        EditorPrefs.SetString(KEY_RELEASE_URL, m_packageUrl);
                    } else {
                        EditorPrefs.SetString(KEY_DEBUG_URL, m_packageUrl);
                    }
                }
                if (GUILayout.Button("GenerateVersion", GUILayout.Height(17))) {
                    GenerateVersion();
                }
                if (GUILayout.Button("CheckVersion", GUILayout.Height(17))) {
                    CheckVersion();
                }
                var rootPath = Path.Combine(Environment.CurrentDirectory, "RemoteAssets");
                EditorGUILayout.LabelField(string.Format("RemoteAssetRoot:{0}", rootPath));

                if (GUILayout.Button("CopyVersionFilesToRemote", GUILayout.Height(17))) {
                    CopyVersionFiles();
                }
                if (GUILayout.Button("OpenRemoteAssetRoot", GUILayout.Height(17))) {
                    Application.OpenURL(rootPath);
                }
                if (GUILayout.Button("OpenPersistantAssetRoot", GUILayout.Height(17))) {
                    Application.OpenURL(Application.persistentDataPath);
                }
            }
            EndSettingsBox();
        }

        /// <summary>
        /// 复制版本文件
        /// </summary>
        private void CopyVersionFiles() {
            var rootPath = Path.Combine(Environment.CurrentDirectory, "RemoteAssets");
            var destRootPath = Path.Combine(rootPath, PathUtility.GetAssetPlatformRoot());
            var sourceRootPath = PathUtility.GetStreamingAssetPlatformPath();

            var sourceVersionPath = PathUtility.GetStreamingVersionPath();
            var destVersionPath = Path.Combine(destRootPath, PathUtility.GetVersionFileName());

            if (!File.Exists(sourceVersionPath)) {
                Debug.Log("版本信息未生成,请先生成版本信息文件！");
                return;
            }
            if (!Directory.Exists(destRootPath)) {
                Directory.CreateDirectory(destRootPath);
            } else {
                Directory.Delete(destRootPath, true);
                Directory.CreateDirectory(destRootPath);
            }
            File.Copy(sourceVersionPath, destVersionPath, true);

            CopyVersionFiles(sourceRootPath, destRootPath);
            Debug.Log("复制完成！");
        }

        public void CopyVersionFiles(string sourceFolder, string destFolder) {
            try {
                //如果目标路径不存在,则创建目标路径
                if (!System.IO.Directory.Exists(destFolder)) {
                    System.IO.Directory.CreateDirectory(destFolder);
                }
                //得到原文件根目录下的所有文件
                string[] files = System.IO.Directory.GetFiles(sourceFolder);
                foreach (string file in files) {
                    var exten = Path.GetExtension(file);
                    if (exten.Equals(".meta")) {
                        continue;
                    }
                    if (exten.Equals(".manifest")) {
                        continue;
                    }
                    string name = System.IO.Path.GetFileName(file);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    System.IO.File.Copy(file, dest, true);//复制文件
                }
                //得到原文件根目录下的所有文件夹
                string[] folders = System.IO.Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders) {
                    string name = System.IO.Path.GetFileName(folder);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    CopyVersionFiles(folder, dest);//构建目标路径,递归复制文件
                }

            } catch (Exception e) {
                Debug.LogError(e.ToString());
            }
        }

        /// <summary>
        /// 剪切
        /// </summary>
        private void CutAndroidAssetBundleToTemp() {
            var rootPath = Path.Combine(Environment.CurrentDirectory, "StreamingAssets_Temp");
            var tempRootPath = Path.Combine(rootPath, PathUtility.GetAndroidAssetRoot());
            var assetRootPath = PathUtility.GetStreamingAssetAndroidPath();

            if (!Directory.Exists(assetRootPath)) {
                Debug.Log($"资源目录不存在：{assetRootPath} 。");
                return;
            }

            if (!Directory.Exists(tempRootPath)) {
                Directory.CreateDirectory(tempRootPath);
            } else {
                Directory.Delete(tempRootPath, true);
                Directory.CreateDirectory(tempRootPath);
            }

            CopyFiles(assetRootPath, tempRootPath);

            Directory.Delete(assetRootPath, true);
            AssetDatabase.Refresh();
            Debug.Log("剪切完成！");
        }

        private void CopyAndroidAssetBundleFormTemp() {
            var rootPath = Path.Combine(Environment.CurrentDirectory, "StreamingAssets_Temp");
            var tempRootPath = Path.Combine(rootPath, PathUtility.GetAndroidAssetRoot());
            var assetRootPath = PathUtility.GetStreamingAssetAndroidPath();

            if (Directory.Exists(assetRootPath)) {
                Debug.Log($"资源目录已存在, 操作取消：{assetRootPath} 。");
                return;
            }

            if (!Directory.Exists(tempRootPath)) {
                Debug.Log($"临时目录不存在：{tempRootPath} 。");
                return;
            }

            if (!Directory.Exists(assetRootPath)) {
                Directory.CreateDirectory(assetRootPath);
            } 

            CopyFiles(tempRootPath,assetRootPath);

            AssetDatabase.Refresh();
            Debug.Log("复制完成！");
        }

        private void CutIosAssetBundleToTemp() {
            var rootPath = Path.Combine(Environment.CurrentDirectory, "StreamingAssets_Temp");
            var tempRootPath = Path.Combine(rootPath, PathUtility.GetIosAssetRoot());
            var assetRootPath = PathUtility.GetStreamingAssetIosPath();

            if (!Directory.Exists(assetRootPath)) {
                Debug.Log($"资源目录不存在：{assetRootPath}。");
                return;
            }

            if (!Directory.Exists(tempRootPath)) {
                Directory.CreateDirectory(tempRootPath);
            } else {
                Directory.Delete(tempRootPath, true);
                Directory.CreateDirectory(tempRootPath);
            }

            CopyFiles(assetRootPath, tempRootPath);

            Directory.Delete(assetRootPath, true);
            AssetDatabase.Refresh();
            Debug.Log("剪切完成！");
        }

        private void CopyIosAssetBundleFormTemp() {
            var rootPath = Path.Combine(Environment.CurrentDirectory, "StreamingAssets_Temp");
            var tempRootPath = Path.Combine(rootPath, PathUtility.GetIosAssetRoot());
            var assetRootPath = PathUtility.GetStreamingAssetIosPath();

            if (Directory.Exists(assetRootPath)) {
                Debug.Log($"资源目录已存在, 操作取消：{assetRootPath} 。");
                return;
            }

            if (!Directory.Exists(tempRootPath)) {
                Debug.Log($"临时目录不存在：{tempRootPath} 。");
                return;
            }

            if (!Directory.Exists(assetRootPath)) {
                Directory.CreateDirectory(assetRootPath);
            }

            CopyFiles(tempRootPath, assetRootPath);

            AssetDatabase.Refresh();
            Debug.Log("复制完成！");
        }

        private void CopyFiles(string sourceFolder, string destFolder) {
            try {
                //如果目标路径不存在,则创建目标路径
                if (!System.IO.Directory.Exists(destFolder)) {
                    System.IO.Directory.CreateDirectory(destFolder);
                }
                //得到原文件根目录下的所有文件
                string[] files = System.IO.Directory.GetFiles(sourceFolder);
                foreach (string file in files) {
                    var exten = Path.GetExtension(file);
                    string name = System.IO.Path.GetFileName(file);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    System.IO.File.Copy(file, dest, true);//复制文件
                }
                //得到原文件根目录下的所有文件夹
                string[] folders = System.IO.Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders) {
                    string name = System.IO.Path.GetFileName(folder);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    CopyFiles(folder, dest);//构建目标路径,递归复制文件
                }

            } catch (Exception e) {
                Debug.LogError(e.ToString());
            }
        }

        public Manifest GetOldVerion() {
            string versionPath = PathUtility.GetStreamingVersionPath();
            Manifest oldVersion = null;
            if (File.Exists(versionPath)) {
                using (StreamReader sr = new StreamReader(new FileStream(versionPath, FileMode.Open))) {
                    var versionInfo = sr.ReadToEnd();
                    oldVersion = LitJson.JsonMapper.ToObject<Manifest>(versionInfo);
                }
            }
            return oldVersion;
        }

        public void GenerateVersion() {

            Debug.Log("生成版本信息:" + m_packageVersion);
            string versionPath = PathUtility.GetStreamingVersionPath();
            if (string.IsNullOrEmpty(versionPath)) {
                throw new ArgumentException("filename Can not be null or Empty!");
            }
            var manifest = new Manifest();
            manifest.version = m_packageVersion;
            manifest.packageUrl = m_packageUrl;

            var directoryPath = PathUtility.GetStreamingAssetPlatformPath();
            WriteFileInfo(directoryPath, directoryPath, manifest);


            if (File.Exists(versionPath)) {
                File.Delete(versionPath);
            }
            using (FileStream fs = new FileStream(versionPath, FileMode.Create)) {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(LitJson.JsonMapper.ToJson(manifest));
                fs.Write(data, 0, data.Length);
            }
            AssetDatabase.Refresh();
        }

        private void WriteFileInfo(string assetPath, string directoryPath, Manifest mainifest) {
            if (!Directory.Exists(directoryPath)) {
                return;
            }
            DirectoryInfo dir = new DirectoryInfo(directoryPath);
            FileSystemInfo[] fileInfos = dir.GetFileSystemInfos();
            for (int i = 0; i < fileInfos.Length; i++) {
                var fileInfo = fileInfos[i];
                if (Directory.Exists(fileInfo.FullName)) {
                    WriteFileInfo(assetPath, fileInfo.FullName, mainifest);
                } else {
                    if (fileInfo.Extension.Equals(".meta") || fileInfo.Extension.Equals(".")) {
                        continue;
                    }
                    if (fileInfo.Extension.Equals(".manifest")) {
                        continue;
                    }
                    var md5 = MD5Utility.GetMD5HashFromFile(fileInfo.FullName);
                    var asset = new AssetInfo();
                    var detailFileInfo = new FileInfo(fileInfo.FullName);
                    asset.size = (int)detailFileInfo.Length;
                    asset.md5 = md5;
                    var fileName = fileInfo.FullName.Substring(assetPath.Length + 1);
                    fileName = fileName.Replace(Path.DirectorySeparatorChar, '/');
                    mainifest.assets.Add(fileName, asset);
                }
            }
        }

        /// <summary>
        /// 比较远程与streaming资源 区别
        /// </summary>
        private void CheckVersion() {
            var localManifest = this.GetOldVerion();

            var versionUrl = localManifest.packageUrl + "/" + PathUtility.GetAssetPlatformRoot() + "/" + PathUtility.GetVersionFileName();
            var versionTempPath = Path.Combine(PathUtility.GetPersistentPath(), PathUtility.GetVersionFileName());
            if (File.Exists(versionTempPath)) {
                File.Delete(versionTempPath);
            }
            this.Download(versionUrl, versionTempPath, (err, progress) => {
                if (err != null) {
                    Debug.LogWarning(string.Format("url:{0} ,err:{1}", versionUrl, err));
                } else if (progress == 1) {
                    this.OnVersionCheck(localManifest);
                }
            });
        }

        /// <summary>
        /// 比较与远程版本
        /// </summary>
        private void OnVersionCheck(Manifest localManifest) {
            var versionTempPath = Path.Combine(PathUtility.GetPersistentPath(), PathUtility.GetVersionFileName());
            Manifest remoteManifest = null;
            using (StreamReader sr = new StreamReader(File.OpenRead(versionTempPath), Encoding.UTF8)) {
                var versionInfo = sr.ReadToEnd();
                try {
                    remoteManifest = LitJson.JsonMapper.ToObject<Manifest>(versionInfo);
                } catch(Exception e) {
                    Debug.Log("remove version manifest error,请检查远程清单文件");
                }
            }
            if (remoteManifest == null) {
                Debug.Log("no remote version manifest");
                return;
            }
            var changeAssets = new List<string>();
            var builder = new StringBuilder();
            foreach (var key in localManifest.assets.Keys) {
                var assetInfo = localManifest.assets[key];
                if (remoteManifest.assets.ContainsKey(key)) {
                    var remoteAssetInfo = remoteManifest.assets[key];
                    if (assetInfo.md5 != remoteAssetInfo.md5) {
                        changeAssets.Add(key);
                        builder.Append("\r\n");
                        builder.Append(string.Format("{0} md5: {1} md5old: {2}", key, assetInfo.md5, remoteAssetInfo.md5));
                    }
                } else {
                    changeAssets.Add(key);
                    builder.Append("\r\n");
                    builder.Append(string.Format("{0} new file", key));
                }
            }
            Debug.Log($"本地版本：{localManifest.version} 资源数量：{localManifest.assets.Count} 远程版本：{remoteManifest.version}  变化资源：{changeAssets.Count} {builder.ToString()}");
            Debug.Log("RemoteUrl:" + remoteManifest.packageUrl);
        }

        public void Download(string url, string filePath, Action<string, float> onResponse) {
            var request = UnityWebRequest.Get(url);
            StartDownloadRequest(request, filePath, onResponse);
        }

        private void StartDownloadRequest(UnityWebRequest request, string filePath, Action<string, float> onResponse) {
            request.chunkedTransfer = true;
            request.timeout = 10000;

            var op = request.SendWebRequest();
            while (!op.isDone) {
            }
            if (!request.isNetworkError) {
                // 判断是否存在路径
                var pathRoot = filePath.Substring(0, filePath.LastIndexOf(Path.DirectorySeparatorChar));
                if (!Directory.Exists(pathRoot)) {
                    Directory.CreateDirectory(pathRoot);
                }
                var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                var downloadHandlerData = request.downloadHandler.data;
                fs.Write(downloadHandlerData, 0, downloadHandlerData.Length);
                fs.Flush();
                fs.Close();
                if (onResponse != null) {
                    onResponse(null, 1);
                }
            } else {
                if (onResponse != null) {
                    onResponse(request.error, 0);
                }
            }
            request.Dispose();
        }

        public void BeginSettingsBox(GUIContent header) {
            bool enabled = GUI.enabled;
            var boxStyle = new GUIStyle("CN Box");
            boxStyle.stretchHeight = false;
            //boxStyle.padding = new RectOffset(0, 0, 0, 0);
            boxStyle.margin = new RectOffset(4, 4, 4, 4);
            EditorGUILayout.BeginVertical(boxStyle);
            var boxHeaderStyle = new GUIStyle("CN Box");
            boxHeaderStyle.stretchHeight = false;
            boxHeaderStyle.padding = new RectOffset(0, 0, 0, 0);
            EditorGUILayout.BeginVertical(boxHeaderStyle);
            var headerStyle = new GUIStyle();
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.border = new RectOffset(3, 3, 3, 3);
            headerStyle.normal.textColor = Color.green;
            EditorGUILayout.LabelField(header, headerStyle);
            EditorGUILayout.EndVertical();
        }

        public void EndSettingsBox() {
            EditorGUILayout.EndVertical();
        }
    }


    /// <summary>
    ///   
    /// </summary>
    [CustomPropertyDrawer(typeof(List<object>), true)]
    public class ListObjectDrawer : PropertyDrawer {
        private ReorderableList m_ReorderableList;

        private void Init(SerializedProperty property) {

            if (m_ReorderableList != null)
                return;
            m_ReorderableList = new ReorderableList(property.serializedObject, property);
            m_ReorderableList.drawElementCallback = DrawOptionData;
            m_ReorderableList.drawHeaderCallback = DrawHeader;
            //m_ReorderableList.elementHeight += 16;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            Init(property);

            m_ReorderableList.DoList(position);
        }

        public void OnGUI(SerializedProperty property) {
            Init(property);
            Rect rect = EditorGUILayout.GetControlRect(false, m_ReorderableList.GetHeight());
            m_ReorderableList.DoList(rect);
        }

        private void DrawHeader(Rect rect) {
            GUI.Label(rect, "BaseAssets");

        }

        private void DrawOptionData(Rect rect, int index, bool isActive, bool isFocused) {
            SerializedProperty itemData = m_ReorderableList.serializedProperty.GetArrayElementAtIndex(index);

            RectOffset offset = new RectOffset(0, 0, -1, -3);
            rect = offset.Add(rect);
            rect.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(rect, itemData, GUIContent.none);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            Init(property);

            return m_ReorderableList.GetHeight();
        }
    }
}