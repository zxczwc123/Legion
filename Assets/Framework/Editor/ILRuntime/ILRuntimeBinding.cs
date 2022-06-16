// ========================================================
// 描 述：ILRuntimeBinding.cs 
// 作 者： 
// 时 间：2020/05/07 21:55:48 
// 版 本：2019.2.1f1 
// ========================================================
using Framework.Utility;
using UnityEditor;

/// <summary>
/// 不能跨越泛型继承
/// </summary>
public class ILRuntimeBinding {
    [MenuItem ("ILRuntime/Generate CLR Binding Code by Analysis")]
    static void GenerateCLRBindingByAnalysis () {
        //用新的分析热更dll调用引用来生成绑定代码
        ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain ();
        var dllPath = PathUtility.GetStreamingHotFixDllPath();
        using (System.IO.FileStream fs = new System.IO.FileStream (dllPath, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
            domain.LoadAssembly (fs);

            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(domain);
            //Cross bind Adapter is needed to generate the correct binding code
            InitILRuntime (domain);
            ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode (domain, "Assets/Framework/ILRuntime/Generated");
        }

        AssetDatabase.Refresh ();


    }

    static void InitILRuntime (ILRuntime.Runtime.Enviorment.AppDomain domain) {
        //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
        domain.RegisterCrossBindingAdaptor (new UnityEventAdapter ());
        domain.RegisterCrossBindingAdaptor (new CoroutineAdapter ());
        

        // domain.RegisterValueTypeBinder (typeof (Vector3), new Vector3Binder ());
    }
}