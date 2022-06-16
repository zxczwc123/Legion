// ========================================================
// 描 述：ResManager.cs 
// 作 者：zxc
// 时 间：2017/05/01 13:42:42 
// 版 本：5.5.2f1 
// ========================================================

using System;
using System.Collections;
using System.IO;
using Framework.Utility;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.Core {
    /// <summary>
    /// 资源管理器
    /// 本工程资源默认都从AssetBundle中加载
    /// 资源存放目录默认放置在 Assets/Game/Res目录下
    /// </summary>
    public class ResManager : Manager<ResManager> {

        private Dictionary<string, AssetBundle> m_loadedAssetBundles = new Dictionary<string, AssetBundle>();

        private AssetBundleManifest m_AssetManifest;

        private AssetBundleManifest assetManifest {
            get {
                if (m_AssetManifest == null) {
                    LoadAssetBundleManifest();
                }
                return m_AssetManifest;
            }
        }

        protected override void Init() {
            base.Init();
        }

        private void LoadAssetBundleManifest() {
            var path = PathUtility.GetAssetsPlatformPath(PathUtility.GetAssetPlatformRoot());
            AssetBundle manifestBundle = AssetBundle.LoadFromFile(path);
            m_AssetManifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");
        }

        private void LoadAllDependencies(string assetBundleName) {
            var bundles = assetManifest.GetAllDependencies(assetBundleName);
            foreach (var bundle in bundles) {
                LoadAllDependencies(bundle);
                if (m_loadedAssetBundles.ContainsKey(bundle.ToLower())) {
                    continue;
                } else {
                    var assetBundlePath = PathUtility.GetAssetsPlatformPath(bundle);
                    var assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
                    Debug.Log(string.Format("assetBundle load : {0}", bundle));
                    m_loadedAssetBundles.Add(bundle.ToLower(), assetBundle);
                }
            }
        }

        private IEnumerator LoadAllDependenciesAsync(string assetBundleName) {
            var bundles = assetManifest.GetAllDependencies(assetBundleName);
            foreach (var bundle in bundles) {
                yield return LoadAllDependenciesAsync(bundle);
                if (m_loadedAssetBundles.ContainsKey(bundle.ToLower())) {
                    if(m_loadedAssetBundles[bundle.ToLower()] == null) {
                        yield return m_loadedAssetBundles[bundle.ToLower()] != null;
                    }
                } else {
                    var assetBundlePath = PathUtility.GetAssetsPlatformPath(bundle);
                    var bundleLoadRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
                    m_loadedAssetBundles.Add(bundle.ToLower(), null);
                    
                    yield return bundleLoadRequest.isDone;
                    var assetBundle = bundleLoadRequest.assetBundle;
                    Debug.Log(string.Format("assetBundle load : {0}", bundle));
                    m_loadedAssetBundles[bundle.ToLower()] = assetBundle;
                }
            }
        }

        /// <summary>
        /// 加载在Resources目录下的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        private T LoadFormResources<T>(string path) where T : UnityEngine.Object {
            T t = Resources.Load<T>(path);
            return t;
        }

        public IEnumerator LoadFromResourcesAsync<T>(string path, ResourcesHolder<T> resourceHolder) where T : UnityEngine.Object {
            ResourceRequest request = Resources.LoadAsync(path);
            yield return request.isDone;
            resourceHolder.asset = ((GameObject)request.asset).GetComponent<T>();
        }

#if UNITY_EDITOR
        /// <summary>
        /// 加载在 Assets/Game/Res目录下的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        private T LoadFormRes<T>(string path) where T : UnityEngine.Object {
            string directoryPath = "";
            string filename = "";
            if (path.Contains("/")) {
                directoryPath = Path.Combine(Application.dataPath, PathUtility.ResPath, path.Substring(0, path.LastIndexOf("/")));
                filename = path.Substring(path.LastIndexOf("/", StringComparison.Ordinal) + 1);
            } else {
                directoryPath = Path.Combine(Application.dataPath, PathUtility.ResPath);
                filename = path;
            }
            var files = new List<string>();
            var fs =  Directory.GetFiles(directoryPath, filename + ".*", SearchOption.TopDirectoryOnly)
                    .Where((r) => !r.EndsWith(".meta"));
            files.AddRange(fs);
           
            if(files.Count < 1) {
                Debug.LogWarning(string.Format("not exists file: {0}", path));
                return null;
            }else if (files.Count > 1) {
                Debug.LogWarning(string.Format("duplicate file: {0}", path));
            }
            var extension = files[0].Extension();
            var filePath = Path.Combine("Assets", PathUtility.ResPath, path);
            T t = AssetDatabase.LoadAssetAtPath<T>(filePath + extension);
            return t;
        }
#endif

        /// <summary>
        /// 加载资源 非编辑状态默认加载assetbundle
        /// 编辑状态下看设置加载 
        ///  T 不能是Transform 或者RectTransform
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T Load<T>(string path) where T : UnityEngine.Object {
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
#if UNITY_EDITOR
            if (Engine.appSettings.isAssetBundle) {
                return LoadFromAssetBundle<T>(path);
            } else {
                return LoadFormRes<T>(path);
            }
#else
            return LoadFromAssetBundle<T>(path);
#endif
        }

        /// <summary>
        ///  T 不能是Transform 或者RectTransform
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="resourseHolder"></param>
        /// <returns></returns>
        public IEnumerator LoadAsync<T>(string path, ResourcesHolder<T> resourseHolder) where T : UnityEngine.Object {
            if (Engine.appSettings.isAssetBundle) {
                // assetbundle打包后资源路径全部为小写
                string assetBundleName = null;
                string assetName = null;
                if (path.Contains("/")) {
                    if (path.Contains("Prefabs")) {
                        assetBundleName = path;
                        assetName = path.Substring(path.LastIndexOf("/") + 1);
                    } else {
                        assetBundleName = path.Substring(0, path.LastIndexOf("/"));
                        assetName = path.Substring(path.LastIndexOf("/") + 1);
                    }
                } else {
                    assetBundleName = path;
                    assetName = path;
                }
                assetBundleName = assetBundleName.ToLower() + PathUtility.AssetBundleExtension;
                AssetBundle assetBundle;
                if (m_loadedAssetBundles.ContainsKey(assetBundleName)) {
                    assetBundle = m_loadedAssetBundles[assetBundleName];
                } else {
                    yield return LoadAllDependenciesAsync(assetBundleName);
                    var assetbundlePath = PathUtility.GetAssetsPlatformPath(assetBundleName);
                    var bundleLoadRequest = AssetBundle.LoadFromFileAsync(assetbundlePath);
                    yield return bundleLoadRequest;
                    Debug.Log(string.Format("assetBundle load : {0}", assetBundleName));
                    assetBundle = bundleLoadRequest.assetBundle;
                    m_loadedAssetBundles.Add(assetBundleName, assetBundle);
                }
                var asset = assetBundle.LoadAsset<T>(assetName);
                resourseHolder.asset = asset;
            } else {
                resourseHolder.asset = Load<T>(path);
            }
        }

        /// <summary>
        /// T 不能是Transform 或者RectTransform
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        private T LoadFromAssetBundle<T>(string path) where T : UnityEngine.Object {
            string assetBundleName = null;
            string assetName = null;
            if (path.Contains("/")) {
                if (path.Contains("Prefabs")) {
                    assetBundleName = path;
                    assetName = path.Substring(path.LastIndexOf("/") + 1);
                } else {
                    assetBundleName = path.Substring(0, path.LastIndexOf("/"));
                    assetName = path.Substring(path.LastIndexOf("/") + 1);
                }
            } else {
                assetBundleName = path;
                assetName = path;
            }
            assetBundleName = assetBundleName.ToLower() + PathUtility.AssetBundleExtension;
            AssetBundle assetBundle;
            if (m_loadedAssetBundles.ContainsKey(assetBundleName)) {
                assetBundle = m_loadedAssetBundles[assetBundleName];
            } else {
                LoadAllDependencies(assetBundleName);
                var assetbundlePath = PathUtility.GetAssetsPlatformPath(assetBundleName);
                assetBundle = AssetBundle.LoadFromFile(assetbundlePath);
                Debug.Log(string.Format("assetBundle load : {0}", assetBundleName));
                m_loadedAssetBundles.Add(assetBundleName, assetBundle);
            }
            var asset = assetBundle.LoadAsset<T>(assetName);
            return asset;
        }

        private bool IsAssetBundleLoaded(string name) {
            var allLoadedAssetBundles = AssetBundle.GetAllLoadedAssetBundles();
            foreach (AssetBundle assetBundle in allLoadedAssetBundles) {
                if (assetBundle.name == name) {
                    return true;
                }
            }
            return false;
        }

        public void OnDestroy() {
            m_loadedAssetBundles.Clear();
            AssetBundle.UnloadAllAssetBundles(true);
        }

        public override void Dispose() {

        }
    }

    public class ResourcesHolder<T> {
        public T asset;
    }
}
