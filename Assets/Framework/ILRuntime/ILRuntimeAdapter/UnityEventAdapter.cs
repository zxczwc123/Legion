// ========================================================
// 描 述：UnityEventAdapter.cs 
// 作 者： 
// 时 间：2020/05/07 22:27:09 
// 版 本：2019.2.1f1 
// ========================================================
using System;
using System.Collections;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using Framework.Core;

public class UnityEventAdapter : CrossBindingAdaptor{
    //定义访问方法的方法信息

    public override Type BaseCLRType {
        get {
            return typeof (UnityEngine.Events.UnityEvent); //这里是你想继承的类型
        }
    }

    public override Type AdaptorType {
        get {
            return typeof (Adapter);
        }
    }

    public override object CreateCLRInstance (ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
        return new Adapter (appdomain, instance);
    }

    public class Adapter : UnityEngine.Events.UnityEvent, CrossBindingAdaptorType {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        //必须要提供一个无参数的构造函数
        public Adapter () {

        }

        public Adapter (ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance { get { return instance; } }

        //下面将所有虚函数都重载一遍，并中转到热更内
        

        public override string ToString () {
            IMethod m = appdomain.ObjectType.GetMethod ("ToString", 0);
            m = instance.Type.GetVirtualMethod (m);
            if (m == null || m is ILMethod) {
                return instance.ToString ();
            } else
                return instance.Type.FullName;
        }

    }
}
