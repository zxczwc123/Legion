// ========================================================
// 描 述：ModuleGenerator.cs 
// 作 者：郑贤春 
// 时 间：2017/05/14 16:11:30 
// 版 本：5.5.2f1 
// ========================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.MEditor
{
    /// <summary>
    /// 生成数据结构目录
    /// 生成基本的类结构
    /// </summary>
    public class ModuleScriptGenerator
    {

        private string moduleScripteTemplates = @"
using Framework.Core;
using UnityEngine;
using UnityEngine.UI;

public class #MODULE# : Module ,I#MODULE#Presenter
{
    public override void OnCreate(Bundle bundle)
    {
       
    }

    public override void OnViewCreate(Transform transfoem)
    {
        #MODULE#View view = new #MODULE#View(transform);
        view.SetPresenter(this);
    }
}

public interface I#MODULE#Presenter
{
    
}

public class #MODULE#View : PresenterView<I#MODULE#Presenter>
{
    public #MODULE#View(Transform transform) : base(transform)
    {
        
    }
}";
        private string m_moduleScript;

        public ModuleScriptGenerator(string moduleName)
        {
            this.m_moduleScript = this.moduleScripteTemplates.Replace("#MODULE#", moduleName);
        }

        public override string ToString()
        {
            return this.m_moduleScript;
        }
    }
}
