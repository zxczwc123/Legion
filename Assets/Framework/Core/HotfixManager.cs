// ========================================================
// 描 述：HotfixManager.cs 
// 作 者： 
// 时 间：2020/05/07 21:41:29 
// 版 本：2019.2.1f1 
// ========================================================
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ILRuntime.Runtime.Intepreter;
using Framework.Utility;
using UnityEngine;

namespace Framework.Core {

    public class HotfixManager : Manager<HotfixManager> {

        public ILRuntime.Runtime.Enviorment.AppDomain AppDomain {
            get {
                return m_appDomain;
            }
        }

        private ILRuntime.Runtime.Enviorment.AppDomain m_appDomain;

        private Assembly m_hotfixAssembly;

        public IEnumerator LoadHotFixAssembly() {
            //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
            m_appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            //正常项目中应该是自行从其他地方下载dll，或者打包在AssetBundle中读取，平时开发以及为了演示方便直接从StreammingAssets中读取，
            //正式发布的时候需要大家自行从其他地方读取dll

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //这个DLL文件是直接编译HotFix_Project.sln生成的，已经在项目中设置好输出目录为StreamingAssets，在VS里直接编译即可生成到对应目录，无需手动拷贝
            //工程目录在Assets\Samples\ILRuntime\1.6\Demo\HotFix_Project~
            //以下加载写法只为演示，并没有处理在编辑器切换到Android平台的读取，需要自行修改
            var dllPath = "";
            if (FrameworkEngine.Instance.isHotFix) {
                HandleVersionAssets();
#if UNITY_ANDROID
                dllPath = PathUtility.GetAssetsPlatformPath(PathUtility.GetHotFixDllFileName());

                if (File.Exists(PathUtility.GetPersisentHotFixDllPath())) {
                    dllPath = "file:///" + dllPath;
                    Debug.Log(PathUtility.GetPersisentHotFixDllPath());
                } else {
                    Debug.Log(PathUtility.GetPersisentHotFixDllPath() + " NOT EXISTS");
                }
#elif UNITY_IOS
                dllPath = "file://" + PathUtility.GetAssetsPlatformPath (PathUtility.GetHotFixDllFileName());
#else
                dllPath = "file:///" + PathUtility.GetAssetsPlatformPath(PathUtility.GetHotFixDllFileName());
#endif
            } else {
#if UNITY_ANDROID
                dllPath = PathUtility.GetStreamingHotFixDllPath();
#elif UNITY_IOS
                dllPath = "file://" + PathUtility.GetStreamingHotFixDllPath();
#else
                dllPath = "file:///" + PathUtility.GetStreamingHotFixDllPath ();
#endif
            }
            WWW www = new WWW(dllPath);
            while (!www.isDone)
                yield return null;
            if (!string.IsNullOrEmpty(www.error))
                UnityEngine.Debug.LogError(www.error);
            byte[] dll = www.bytes;
            www.Dispose();

            //         //PDB文件是调试数据库，如需要在日志中显示报错的行号，则必须提供PDB文件，不过由于会额外耗用内存，正式发布时请将PDB去掉，下面LoadAssembly的时候pdb传null即可
            // #if UNITY_ANDROID
            //         www = new WWW(Application.streamingAssetsPath + "/HotFix_Project.pdb");
            // #else
            //         www = new WWW("file:///" + Application.streamingAssetsPath + "/HotFix_Project.pdb");
            // #endif
            //         while (!www.isDone)
            //             yield return null;
            // if (!string.IsNullOrEmpty(www.error))
            //     UnityEngine.Debug.LogError(www.error);
            // byte[] pdb = www.bytes;
            // using(var fs = new MemoryStream(dll)){
            var fs = new MemoryStream(dll);
            m_appDomain.LoadAssembly(fs);
            // }

            InitializeILRuntime();
        }

        /// <summary>
        /// 处理版本  当本地版本比包体版本低的时候进行清除
        /// </summary>
        private void HandleVersionAssets() {
            new VersionHandler().HandleAssets();
        }

        void InitializeILRuntime() {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            m_appDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(m_appDomain);

            //这里做一些ILRuntime的注册，HelloWorld示例暂时没有需要注册的
            m_appDomain.RegisterCrossBindingAdaptor(new ModuleAdapter());
            m_appDomain.RegisterCrossBindingAdaptor(new ViewAdapter());
            m_appDomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
            m_appDomain.RegisterCrossBindingAdaptor(new UnityEventAdapter());

            //委托注册

            m_appDomain.DelegateManager.RegisterFunctionDelegate<int>();
            m_appDomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, bool>();
            m_appDomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.String>();
            m_appDomain.DelegateManager.RegisterFunctionDelegate<System.Boolean>();            //m_appDomain.DelegateManager.RegisterFunctionDelegate<System.Exception, System.Threading.Tasks.Task>();
            m_appDomain.DelegateManager.RegisterMethodDelegate<int>();
            m_appDomain.DelegateManager.RegisterMethodDelegate<bool>();
            m_appDomain.DelegateManager.RegisterMethodDelegate<float>();
            m_appDomain.DelegateManager.RegisterMethodDelegate<long>();
            m_appDomain.DelegateManager.RegisterMethodDelegate<string>();
            m_appDomain.DelegateManager.RegisterMethodDelegate<int, int>();
            m_appDomain.DelegateManager.RegisterMethodDelegate<float, float>();
            m_appDomain.DelegateManager.RegisterMethodDelegate<string, string>();
            m_appDomain.DelegateManager.RegisterMethodDelegate<string, float>();
            m_appDomain.DelegateManager.RegisterMethodDelegate<float, string>();
            m_appDomain.DelegateManager.RegisterMethodDelegate<int, string>();
            m_appDomain.DelegateManager.RegisterMethodDelegate<string, int>();
            m_appDomain.DelegateManager.RegisterMethodDelegate<Framework.Core.Bundle>();
            m_appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.EventSystems.PointerEventData>();
            m_appDomain.DelegateManager.RegisterMethodDelegate<Spine.TrackEntry>();            m_appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector2>();            m_appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector3>();            //m_appDomain.DelegateManager.RegisterMethodDelegate<System.Threading.Tasks.Task>();

            // m_appDomain.DelegateManager.RegisterMethodDelegate<Task<Firebase.DependencyStatus>>();
            m_appDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) => {
                return new UnityEngine.Events.UnityAction(() => {
                    ((Action)act)();
                });
            });
            m_appDomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback>((act) => {
                return new DG.Tweening.TweenCallback(() => {
                    ((Action)act)();
                });
            });

            m_appDomain.DelegateManager.RegisterDelegateConvertor<Spine.AnimationState.TrackEntryDelegate>((act) => {
                return new Spine.AnimationState.TrackEntryDelegate((trackEntry) => {
                    ((Action<Spine.TrackEntry>)act)(trackEntry);
                });
            });

            m_appDomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) => {
                return new System.Predicate<ILRuntime.Runtime.Intepreter.ILTypeInstance>((obj) => {
                    return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean>)act)(obj);
                });
            });
            m_appDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.String>>((act) => {
                return new UnityEngine.Events.UnityAction<System.String>((arg0) => {
                    ((Action<System.String>)act)(arg0);
                });
            });            m_appDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Single>>((act) => {
                return new UnityEngine.Events.UnityAction<System.Single>((arg0) => {
                    ((Action<System.Single>)act)(arg0);
                });
            });
            m_appDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.Vector2>>((act) => {
                return new UnityEngine.Events.UnityAction<UnityEngine.Vector2>((arg0) => {
                    ((Action<UnityEngine.Vector2>)act)(arg0);
                });
            });
            m_appDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.Vector3>>((act) => {
                return new UnityEngine.Events.UnityAction<UnityEngine.Vector3>((arg0) => {
                    ((Action<UnityEngine.Vector3>)act)(arg0);
                });
            });

            m_appDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Boolean>>((act) => {
                return new UnityEngine.Events.UnityAction<System.Boolean>((arg0) => {
                    ((Action<System.Boolean>)act)(arg0);
                });
            });

            m_appDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<int>>((act) => {
                return new UnityEngine.Events.UnityAction<int>((arg0) => {
                    ((Action<int>)act)(arg0);
                });
            });
        }

        public void RegisterMethodDelegate<T>() {
            m_appDomain.DelegateManager.RegisterMethodDelegate<T>();
        }

        public void RegisterMethodDelegate<T1, T2>() {
            m_appDomain.DelegateManager.RegisterMethodDelegate<T1, T2>();
        }

        public void RegisterMethodDelegate<T1, T2, T3>() {
            m_appDomain.DelegateManager.RegisterMethodDelegate<T1, T2, T3>();
        }

        public void RegisterFunctionDelegate<T>() {
            m_appDomain.DelegateManager.RegisterFunctionDelegate<T>();
        }

        public void RegisterFunctionDelegate<T1, T2>() {
            m_appDomain.DelegateManager.RegisterFunctionDelegate<T1, T2>();
        }

        public void RegisterFunctionDelegate<T1, T2, T3>() {
            m_appDomain.DelegateManager.RegisterFunctionDelegate<T1, T2, T3>();
        }

        public object GetObject(string type) {
            if (FrameworkEngine.Instance.isDLL) {
                ILTypeInstance instance = m_appDomain.Instantiate(type);

                if (instance != null) {
                    return instance.CLRInstance;
                }
                return null;
            } else {
                // 当Game 和 Framework 使用assembly define 的时候用
                var assembly = Assembly.Load("Game");
                if (assembly == null) {
                    assembly = this.GetType().Assembly;
                }
                Type objectType = assembly.GetType(type);
                if (objectType == null) {
                    throw new Exception(string.Format("type: {0} can not found", type));
                }
                return Activator.CreateInstance(objectType);
            }
        }

        private void OnDestroy() {

        }

        public override void Dispose() {

        }



    }
}