/*
 * OnionEngine Framework for Unity By wzl
 * -------------------------------------------------------------------
 * Name    : BaseI18nComponent
 * Date    : 2018/10/31
 * Version : v1.0
 * Describe: 
 */
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework.I18n
{
    public abstract class BaseI18nComponent : UIBehaviour
    {
        [SerializeField]
        protected string id;

        internal protected abstract void I18nUpdate();
        
        protected override void Start()
        {
            I18n.OnI18nComponentEnable (this);
            this.I18nUpdate();
        }

        protected override void OnDestroy()
        {
            I18n.OnI18nComponentDisable (this);
        }
    }
}