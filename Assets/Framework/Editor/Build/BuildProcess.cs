// ========================================================
// 描 述：VersionEditor.cs 
// 作 者： 
// 时 间：2020/07/22 21:29:41 
// 版 本：2019.2.1f1 
// ========================================================
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using UnityEngine;

namespace Framework.MEditor {
    public class BuildProcess : IPreprocessBuildWithReport, IPostprocessBuildWithReport {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report) {
            //Debug.Log("打包前");
        }

        public void OnPostprocessBuild(BuildReport report) {
#if UNITY_ANDROID
            Debug.Log("处理 Only fullscreen opaque activities can request orientation 样式问题");
            // 此处 为了解决bug 将 工程里的style 文件替换打包后的style 文件
            var outStyleFile = Path.Combine(report.summary.outputPath, Application.productName, "src/main/res/values/styles.xml");
            var projectStyleFile = Path.Combine(Application.dataPath, "Plugins/Android/res/values/style.xml");
            File.Copy(projectStyleFile, outStyleFile, true);
#elif UNITY_IOS
            // 输出目录同级下 ReplaceFile中的info 替换 打包工程里的info
            string sourceInfoFilePath = report.summary.outputPath.Substring(0, report.summary.outputPath.LastIndexOf("/"));
            string sourceInfoFile = Path.Combine(sourceInfoFilePath, "ReplaceFile/Info.plist");
            if (File.Exists(sourceInfoFile)) {
                string outputInfoFile = System.IO.Path.Combine(report.summary.outputPath, "Info.plist");
                System.IO.File.Copy(sourceInfoFile, outputInfoFile, true);//复制替换文件
            } else {
                Debug.LogWarning($"文件不存在:{sourceInfoFile}");
            }

            // 初始化
            string projPath = PBXProject.GetPBXProjectPath(report.summary.outputPath);
            PBXProject proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(projPath));
            string target = proj.TargetGuidByName("Unity-iPhone");
            //// 添加flag
            //proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
            // 添加framework 此两个会在append的时候消失 重新补充上 Alipay.framework的embed 属性会产生变化需要修改
            proj.AddFrameworkToProject(target, "CoreLocation.framework", false);
            proj.AddFrameworkToProject(target, "StoreKit.framework", false);
            //添加lib
            //AddLibToProject(proj, target, "libc++.tbd");
            //AddLibToProject(proj, target, "libz.tbd");
            // 关闭Bitcode设置
            //proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
            // 打开C语言模块设置
            //proj.SetBuildProperty(target, "CLANG_ENABLE_MODULES", "YES");
            //{
            //    // 修改info.plist的代码
            //    string plistPath = report.summary.outputPath + "/Info.plist";
            //    PlistDocument plist = new PlistDocument();
            //    plist.ReadFromString(File.ReadAllText(plistPath));
            //    PlistElementDict rootDict = plist.root;
            //    //
            //    PlistElement array = null;
            //    if (rootDict.values.ContainsKey("LSApplicationQueriesSchemes")) {
            //        array = rootDict["LSApplicationQueriesSchemes"].AsArray();
            //    } else {
            //        array = rootDict.CreateArray("LSApplicationQueriesSchemes");
            //    }
            //    rootDict.values.TryGetValue("LSApplicationQueriesSchemes", out array);
            //    PlistElementArray Qchemes = array.AsArray();
            //    Qchemes.AddString("fb");
            //    //添加SDK要求接入的权限[权限名,申请说明]
            //    rootDict.SetString("NSBluetoothPeripheralUsageDescription", " Advertisement would like to use bluetooth.");
            //}
            // 应用修改
            File.WriteAllText(projPath, proj.WriteToString());
#endif
        }

#if UNITY_IOS

        //添加lib方法
        //static void AddLibToProject(PBXProject inst, string targetGuid, string lib) {
        //    string fileGuid = inst.AddFile("usr/lib/" + lib, "Frameworks/" + lib, PBXSourceTree.Sdk);
        //    inst.AddFileToBuild(targetGuid, fileGuid);
        //}

#endif
    }
}