// ========================================================
// 描 述：DllCompiler.cs 
// 作 者： 
// 时 间：2020/05/03 16:20:26 
// 版 本：2019.2.1f1 
// ========================================================

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using System;
using System.Reflection;
using Framework.Utility;

namespace Framework.MEditor.DllCompiler
{
    public class DllCompiler : Editor
    {
        [MenuItem("Framework/GenerateDLL", false, 1)]
        public static void GenerateDLL()
        {
            StartCompile();
        }

        private static void StartCompile()
        {
            var codeSourceFolder = $"{Application.dataPath}/Game";
            var compilerFolder = $"{Environment.CurrentDirectory}/Tools/BuildHotfixDll/roslyn";
            //编译dll
            var outputPath = PathUtility.GetStreamingHotFixDllPath();
            var define = GetScriptingDefineSymbols();
            var dllFolders = new List<string>();
            dllFolders.Add($"{EditorApplication.applicationContentsPath}/NetStandard");
            dllFolders.Add($"{EditorApplication.applicationContentsPath}/Managed/UnityEngine");
            dllFolders.Add($"{Environment.CurrentDirectory}/Library/ScriptAssemblies");
            Debug.Log(codeSourceFolder);
            ScriptBuildToDll.Build(codeSourceFolder, outputPath, dllFolders.ToArray(), compilerFolder, define);
        }

        //获取编译选项
        private static string GetScriptingDefineSymbols()
        {
            List<string> validDefines = new List<string>();
            foreach (var define in EditorUserBuildSettings.activeScriptCompilationDefines)
            {
                if (!define.Contains("UNITY_EDITOR"))
                {
                    validDefines.Add(define);
                }
            }
            return string.Join(";", validDefines);
        }
    }

    public class ScriptBuildToDll
    {
        public enum BuildStatus
        {
            Success = 0,
            Fail
        }

        public static void Build(string codeSourceFolder, string outputPath, string[] dllFolders,
            string compilerFolder, string define)
        {
            //编译项目的base.dll
            Debug.Log("准备编译dll 10%");

            //找出所有的脚本
            List<string> files = new List<string>();
            var codeFolderFiles = Directory.GetFiles(codeSourceFolder, "*.*", SearchOption.AllDirectories).ToList();
            var codeFiles = codeFolderFiles.FindAll(f =>
            {
                var file = f.ToLower();
                var extension = Path.GetExtension(file);
                if (!file.Contains("editor") && (extension.Equals(".dll") || extension.Equals(".cs")))
                {
                    return true;
                }
                return false;
            });

            files.AddRange(codeFiles);
            files = files.Distinct().ToList();

            Debug.Log("开始整理script 20%");

            // 项目中用到的dll
            var dllFiles = files.FindAll(f => f.EndsWith(".dll"));
            // unity内脚本，用于先生成unity的dll文件，供hotfix.dll编译用
            var csFiles = files.FindAll(f => !f.EndsWith(".dll") && !f.Contains("@Hotfix"));
            // 热更脚本，用于生成hotfix.dll
            var hotfixCsFiles = files.FindAll(f => !f.EndsWith(".dll"));

            //临时目录
            var tempFolder = Path.Combine(Environment.CurrentDirectory, "/dll_build_temp");
            tempFolder = tempFolder.Replace('\\', '/');
            if (Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
            }
            Directory.CreateDirectory(tempFolder);

            //除去不需要引用的dll
            for (int i = dllFiles.Count - 1; i >= 0; i--)
            {
                var str = dllFiles[i];
                if (str.Contains("Editor") || str.Contains("iOS") || str.Contains("Android") ||
                    str.Contains("StreamingAssets"))
                {
                    dllFiles.RemoveAt(i);
                }
            }

            //拷贝dll到临时目录
            for (int i = 0; i < dllFiles.Count; i++)
            {
                var copyFile = Path.Combine(tempFolder, Path.GetFileName(dllFiles[i]));
                copyFile = copyFile.Replace("\\", "/");
                Debug.Log("编译文件：" + copyFile);
                File.Copy(dllFiles[i], copyFile, true);
                dllFiles[i] = copyFile;
            }

            //添加系统的dll
            dllFiles.Add("System.dll");
            dllFiles.Add("System.Core.dll");
            dllFiles.Add("System.XML.dll");
            dllFiles.Add("System.Data.dll");

            //添加Unity系统的dll
            foreach (string dll in dllFolders)
            {
                var dllFile = Directory.GetFiles(dll, "*.dll", SearchOption.AllDirectories);
                foreach (var file in dllFile)
                {
                    // steamingAssets 去除
                    if (file.Contains("StreamingAssets")) continue;
                    dllFiles.Add(file);
                }
            }

            Debug.Log("复制编译代码 30%");

            //拷贝非热更的cs文件到临时目录
            for (int i = 0; i < csFiles.Count; i++)
            {
                var copyFile = Path.Combine(tempFolder, Path.GetFileName(csFiles[i]));
                copyFile = copyFile.Replace("\\", "/");
                int count = 1;
                while (File.Exists(copyFile))
                {
                    //为解决mono.exe error: 文件名太长问题
                    copyFile = copyFile.Replace(".cs", "") + count + ".cs";
                    count++;
                }

                File.Copy(csFiles[i], copyFile);
                csFiles[i] = copyFile;
            }

            //检测dll，移除无效dll  移除重复
            var singleFileDict = new Dictionary<string,string>();
            for (var i = 0; i < dllFiles.Count; i++)
            {
                var file = dllFiles[i];
                var fileName = file;
                if (fileName.Contains('/'))
                    fileName = file.Substring(file.LastIndexOf('/') + 1);
                if (singleFileDict.ContainsKey(fileName))
                {
                    continue;
                }
                // 判断是否无效
                if (File.Exists(file))
                {
                    var fs = File.ReadAllBytes(file);
                    try
                    {
                        Assembly.Load(fs);
                    }
                    catch
                    {
                        Debug.Log("移除无效的 dll ：" + file);
                        dllFiles.RemoveAt(i);
                    }
                }
                singleFileDict.Add(fileName,file);
            }
            dllFiles = singleFileDict.Values.ToList();

            Debug.Log("[1/2]开始编译 unity.dll 40%");
            
            
            BuildStatus buildResult;
            try
            {
                StringBuilder builder = new StringBuilder();
                foreach (var dll in dllFiles)
                {
                    builder.Append("\r\n" + dll);
                }
                Debug.Log($"所有引用DLL: {builder}");
                buildResult = BuildDll(dllFiles.ToArray(), hotfixCsFiles.ToArray(), outputPath, compilerFolder,
                    define);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                buildResult = BuildStatus.Fail;
            }

            Debug.Log("清理临时文件 95%");
            Directory.Delete(tempFolder, true);

            if (buildResult == BuildStatus.Success)
            {
                Debug.Log("编译成功!");
            }
            else
            {
                Debug.LogError("编译失败!");
            }
        }

        /// <summary>
        /// 编译dll
        /// </summary>
        public static BuildStatus BuildDll(string[] refAssemblies, string[] codeFiles, string output,
            string compilerFolder, string define)
        {
            // 设定编译参数,DLL代表需要引入的Assemblies
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            //在内存中生成
            cp.GenerateInMemory = true;
            //生成调试信息
            if (define.IndexOf("IL_DEBUG", StringComparison.Ordinal) >= 0)
            {
                cp.IncludeDebugInformation = true;
            }
            else
            {
                cp.IncludeDebugInformation = false;
            }

            cp.OutputAssembly = output;
            //warning和 error分开,不然各种warning当成error,改死你
            cp.TreatWarningsAsErrors = false;
            cp.WarningLevel = 1;
            //编译选项
            cp.CompilerOptions = "-langversion:latest /optimize /unsafe /define:" + define;

            if (refAssemblies != null)
            {
                foreach (var d in refAssemblies)
                {
                    cp.ReferencedAssemblies.Add(d);
                }
            }

            // 编译代理
            CodeDomProvider provider;
            if (string.IsNullOrEmpty(compilerFolder))
            {
                provider = CodeDomProvider.CreateProvider("CSharp");
            }
            else
            {
                provider = CodeDomProvider.CreateProvider("cs", new Dictionary<string, string>
                {
                    {"CompilerDirectoryPath", compilerFolder}
                });
            }

            CompilerResults cr = provider.CompileAssemblyFromFile(cp, codeFiles);
            if (cr.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();
                foreach (CompilerError ce in cr.Errors)
                {
                    sb.Append(ce);
                    sb.Append(Environment.NewLine);
                }
                Debug.LogError(sb);
            }
            else
            {
                return BuildStatus.Success;
            }
            return BuildStatus.Fail;
        }
    }
}