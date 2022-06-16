// // ========================================================
// // 描 述：ModuleWindow.cs 
// // 作 者：郑贤春 
// // 时 间：2017/05/14 12:27:00 
// // 版 本：5.5.2f1 
// // ========================================================
// using System.Collections;
// using System.Collections.Generic;
// using Framework.Core;
// using UnityEditor;
// using UnityEngine;
//
// namespace Framework.MEditor {
//     public class ModuleWindow : EditorWindow {
//         //[MenuItem ("Framework/Module/ModuleGenerator", false, 0)]
//         public static void ModuleGenerator () {
//             ModuleGenerateWindow.ShowWindow ();
//         }
//
//         //[MenuItem ("Framework/Module/ModuleSetting", false, 0)]
//         public static void ShowModuleWindow () {
//             ModuleWindow.ShowWindow ();
//         }
//
//         public static void ShowWindow () {
//             ModuleWindow window = EditorWindow.GetWindow<ModuleWindow> (true, "ModuleSetting");
//             window.maxSize = new Vector2 (450, 800);
//             window.minSize = new Vector2 (450, 400);
//         }
//
//         private ModuleConfigHandler m_moduleConfig;
//
//         private ModuleInfo m_moduleToCreate;
//
//         private bool m_createFlag = false;
//
//         void OnEnable () {
//             this.m_moduleConfig = new ModuleConfigHandler();
//             // 重新加载，防止手动更改后不一致
//             this.m_moduleConfig.Reload ();
//         }
//
//         void OnGUI () {
//             if (this.m_moduleConfig == null) {
//                 return;
//             }
//             List<ModuleInfo> moduleList = this.m_moduleConfig.GetModuleList ();
//             if (moduleList == null) {
//                 return;
//             }
//             int width = 100;
//             for (int i = -2; i < moduleList.Count; i++) {
//                 EditorGUILayout.BeginHorizontal (); {
//                     if (i == -2) {
//                         EditorGUILayout.LabelField ("name", GUILayout.Width (width));
//                         EditorGUILayout.LabelField ("moduleType", GUILayout.Width (width));
//                         EditorGUILayout.LabelField ("isPermanent", GUILayout.Width (width));
//                         EditorGUILayout.LabelField ("scene", GUILayout.Width (width));
//                         if (GUILayout.Button ("+")) {
//                             this.m_createFlag = true;
//                             this.m_moduleToCreate = new ModuleInfo ();
//                             this.m_moduleToCreate.name = "";
//                             this.m_moduleToCreate.type = "";
//                             this.m_moduleToCreate.isPermanent = false;
//                             this.m_moduleToCreate.scene = "";
//                         }
//                     } else if (i == -1) {
//                         EditorGUILayout.LabelField ("string", GUILayout.Width (width));
//                         EditorGUILayout.LabelField ("string", GUILayout.Width (width));
//                         EditorGUILayout.LabelField ("bool", GUILayout.Width (width));
//                         EditorGUILayout.LabelField ("string", GUILayout.Width (width));
//                     } else {
//                         EditorGUI.BeginChangeCheck ();
//                         ModuleInfo module = moduleList[i];
//                         module.name = EditorGUILayout.TextField (module.name, GUILayout.Width (width));
//                         module.type = EditorGUILayout.TextField (module.type, GUILayout.Width (width));
//                         module.isPermanent = EditorGUILayout.Toggle (module.isPermanent, GUILayout.Width (width));
//                         module.scene = EditorGUILayout.TextField (module.scene, GUILayout.Width (width));
//                         if (EditorGUI.EndChangeCheck ()) {
//                             this.m_moduleConfig.Save ();
//                         }
//                         if (GUILayout.Button ("-")) {
//                             RemoveModule (moduleList[i].name);
//                         }
//
//                     }
//                 }
//                 EditorGUILayout.EndHorizontal ();
//             }
//             if (this.m_createFlag && this.m_moduleToCreate != null) {
//                 EditorGUILayout.BeginHorizontal (); {
//                     this.m_moduleToCreate.name = EditorGUILayout.TextField (this.m_moduleToCreate.name, GUILayout.Width (width));
//                     this.m_moduleToCreate.type = EditorGUILayout.TextField (this.m_moduleToCreate.type, GUILayout.Width (width));
//                     this.m_moduleToCreate.isPermanent = EditorGUILayout.Toggle (this.m_moduleToCreate.isPermanent, GUILayout.Width (width));
//                     this.m_moduleToCreate.scene = EditorGUILayout.TextField (this.m_moduleToCreate.scene, GUILayout.Width (width));
//                     if (GUILayout.Button ("√")) {
//                         if (this.m_moduleConfig.AddModule (this.m_moduleToCreate)) {
//                             this.m_moduleConfig.Save ();
//                             this.m_createFlag = false;
//                         } else {
//                             EditorUtility.DisplayDialog ("提示", "添加失败！", "确定");
//                         }
//                     }
//                 }
//                 EditorGUILayout.EndHorizontal ();
//             }
//         }
//
//         private void RemoveModule (string moduleName) {
//             this.m_moduleConfig.RomoveModule (moduleName);
//             this.m_moduleConfig.Save ();
//         }
//     }
// }