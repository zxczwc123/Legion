// ========================================================
// 描 述：CsharpGernerator.cs 
// 作 者：郑贤春 
// 时 间：2017/01/02 19:09:31 
// 版 本：5.4.1f1 
// ========================================================
using System;
using System.Collections.Generic;

namespace Framework.MEditor.CodeGenerator
{
    
    public enum AuthorityType
    {
        Default = 0,
        Public,
        Internal,
        Private
    }

    public enum ClassDecorateType
    {
        Default = 0,
        Abstract,
    }

    public enum MethodAuthorityType
    {
        Default = 0,
        Public,
        Internal,
        Private
    }

    public enum MethodDecorateType
    {
        Default = 0,
        Abstract,
        Override,
        Virtual
    }

    public interface IClassGenerator
    {
        void SetNamespace(string name);

        void SetClassName(string name);

        void SetBaseClass(string className);

        void SetInterfaces(string[] interfaceNames);

        void SetUsingName(string[] usingName);

        void SetAuthority(AuthorityType type);

        void SetDecorate(ClassDecorateType type);

        void AddMethod(IMethodGenerator method);
    }

    public interface IMethodGenerator
    {
        void SetMethodName(string name);

        void SetAuthority(MethodAuthorityType type);

        void SetDecorate(MethodDecorateType type);

        void SetParms(string[] parms);

        void SetReturn(string returnType);
    }

    public interface IPropertyGenerator
    {
        void SetPropertyName(string name);

        void SetAuthority(AuthorityType type);
    }

    public interface IFieldGenerator
    {
        void SetFieldName(string name);

        void SetAuthority(AuthorityType type);
    }

    public class ClassGenerator : IClassGenerator
    {
        private const string CONTENT =
@"#UsingNames#
#NamespaceBegin#
#CONTENT#
#NamespaceEnd#";
        private const string MAINCONTENT =
@"#Authority##DecorateType#class #ClassName##ExtendsName#
{
#FieldContent##PropetyContent##MethodContent#
}";
        private const string USING =
@"using #UsingName#;
";
        private const string AUTHORITY = "|public|internal|private";
        private const string DECORATE = "|abstract";

        public static string[] authorityTypes
        {
            get
            {
                return AUTHORITY.Split('|');
            }
        }

        private string[] decorateTypes
        {
            get
            {
                return DECORATE.Split('|');
            }
        }

        private string m_name;
        private string m_baseName;
        private string m_content;
        private string m_mainContent;
        private string[] m_interfaceNames;
        private string[] m_usingNames;
        private string m_namespace;
        private List<IMethodGenerator> m_methods;
        private List<IFieldGenerator> m_fields;
        private List<IPropertyGenerator> m_properties;
        private AuthorityType m_authorityType;
        private ClassDecorateType m_decorateType;

        private ClassGenerator(){ }

        public ClassGenerator(string className)
        {
            this.m_content = CONTENT;
            this.m_mainContent = MAINCONTENT;
            this.m_name = className;
        }

        public void AddMethod(IMethodGenerator method)
        {
            if (this.m_methods == null) this.m_methods = new List<IMethodGenerator>();
            this.m_methods.Add(method);
        }

        public void AddProperty(IPropertyGenerator property)
        {
            if (this.m_properties == null) this.m_properties = new List<IPropertyGenerator>();
            this.m_properties.Add(property);
        }

        public void AddField(IFieldGenerator field)
        {
            if (this.m_fields == null) this.m_fields = new List<IFieldGenerator>();
            this.m_fields.Add(field);
        }

        public void SetAuthority(AuthorityType type)
        {
            this.m_authorityType = type;
        }

        public void SetBaseClass(string baseName)
        {
            this.m_baseName = baseName;
        }

        public void SetClassName(string name)
        {
            this.m_name = name;
        }

        public void SetDecorate(ClassDecorateType type)
        {
            this.m_decorateType = type;
        }

        public void SetInterfaces(string[] interfaceNames)
        {
            this.m_interfaceNames = interfaceNames;
        }

        public void SetNamespace(string name)
        {
            this.m_namespace = name;
        }

        public void SetUsingName(string[] usingNames)
        {
            this.m_usingNames = usingNames;
        }

        public override string ToString()
        {
            this.m_mainContent = this.m_mainContent.Replace("#Authority#", GetAuthority(this.m_authorityType));
            this.m_mainContent = this.m_mainContent.Replace("#DecorateType#", GetDecorate(this.m_decorateType));
            this.m_mainContent = this.m_mainContent.Replace("#ClassName#", this.m_name);
            this.m_mainContent = this.m_mainContent.Replace("#ExtendsName#", GetExtends(this.m_baseName, this.m_interfaceNames));
            this.m_mainContent = this.m_mainContent.Replace("#MethodContent#", GetMethods(this.m_methods));
            this.m_mainContent = this.m_mainContent.Replace("#PropetyContent#", GetProperties(this.m_properties));
            this.m_mainContent = this.m_mainContent.Replace("#FieldContent#", GetFields(this.m_fields));
            if(!IsNamespaceNullOrEmpty())
            {
                this.m_mainContent = RetractContent(this.m_mainContent);
            }

            this.m_content = this.m_content.Replace("#UsingNames#", GetUsingNames(this.m_usingNames));
            this.m_content = this.m_content.Replace("#NamespaceBegin#", GetNamespace(this.m_namespace)[0]);
            this.m_content = this.m_content.Replace("#NamespaceEnd#", GetNamespace(this.m_namespace)[1]);
            this.m_content = this.m_content.Replace("#CONTENT#", this.m_mainContent);
            return this.m_content;
        }

        string RetractContent(string content)
        {
            return "    " + content.Replace("\r\n", "\r\n    "); ;
        }

        string GetUsingNames(string[] names)
        {
            string usingNames = "";
            if(names == null || names.Length < 1)
            {
                return usingNames;
            }
            else
            {
                for(int i=0;i<names.Length;i++)
                {
                    usingNames += USING.Replace("#UsingName#", names[i]);
                }
                return usingNames;
            }
        }

        string[] GetNamespace(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                return new string[] { "", "" };
            }
            string begin = string.Format("namespace {0}{1}", name,"{");
            string end = "}";
            return new string[] { begin, end};
        }

        string GetAuthority(AuthorityType type)
        {
            if(type == AuthorityType.Default)
            {
                return "";
            }
            else
            {
                return authorityTypes[(int)type] + " ";
            }
        }

        string GetDecorate(ClassDecorateType type)
        {
            string decorateType = "";
            if(type == ClassDecorateType.Default)
            {
                return decorateType;
            }
            decorateType = this.decorateTypes[(int)type] + " ";
            return decorateType;
        }

        string GetExtends(string baseName,string[] interfaces)
        {
            if(string.IsNullOrEmpty(baseName) && interfaces == null || interfaces.Length < 1)
            {
                return "";
            }
            else
            {
                string extents = " : ";
                if(!string.IsNullOrEmpty(baseName))
                {
                    extents += string.Format("{0},", baseName);
                }
                if(interfaces != null && interfaces.Length > 0)
                {
                    for(int i=0;i<interfaces.Length;i++)
                    {
                        extents += string.Format("{0},", interfaces[i]);
                    }
                    
                }
                return extents.Substring(0, extents.Length - 1);
            }
        }

        string GetFields(List<IFieldGenerator> fields)
        {
            if (fields == null || fields.Count < 1) return "";
            string fieldValue = "";
            for (int i = 0; i < fields.Count; i++)
            {
                fieldValue += fields[i].ToString();
                if (i != fields.Count - 1)
                {
                    fieldValue += "\r\n\r\n";
                }
                else if(!IsMethodNullOrEmpty() || !IsPropertyNullOrEmpty())
                {
                    fieldValue += "\r\n\r\n";
                }
            }
            return fieldValue;
        }

        string GetProperties(List<IPropertyGenerator> properties)
        {
            if (properties == null || properties.Count < 1) return "";
            string propetyValue = "";
            for (int i = 0; i < properties.Count; i++)
            {
                propetyValue += properties[i].ToString();
                if (i != properties.Count - 1)
                {
                    propetyValue += "\r\n\r\n";
                }
                else if(!IsMethodNullOrEmpty())
                {
                    propetyValue += "\r\n\r\n";
                }
            }
            return propetyValue;
        }

        string GetMethods(List<IMethodGenerator> methods)
        {
            if(methods == null || methods.Count < 1) return "";
            string methodValue = "";
            for(int i=0;i<methods.Count;i++)
            {
                methodValue += methods[i].ToString();
                methodValue = i != methods.Count - 1 ?  methodValue += "\r\n\r\n" : methodValue;
            }
            return methodValue;
        }

        bool IsMethodNullOrEmpty()
        {
            return this.m_methods == null || this.m_methods.Count < 1 ? true : false;
        }

        bool IsPropertyNullOrEmpty()
        {
            return this.m_properties == null || this.m_properties.Count < 1 ? true : false;
        }

        bool IsNamespaceNullOrEmpty()
        {
            return string.IsNullOrEmpty(this.m_namespace);
        }
    }

    public class MethodGenerator : IMethodGenerator
    {
        private const string DECORATE = @"|abstract|override|virtual";
        private const string CONTENT = @"    #Authority##DecorateType##ReturnType# #MethodName#(#Parms#)#MethodContent#";
        private const string METHODCONTENT = 
@"
    {
        #ReturnValue#
    }";

        private string m_content;
        private string m_methodContent;

        private MethodAuthorityType m_authorityType = MethodAuthorityType.Default;

        private MethodDecorateType m_decorateType = MethodDecorateType.Default;

        private string[] m_parms;

        private string m_name;

        private string m_returnType;

        private string[] m_decorateTypes;

        public MethodGenerator(string methodName)
        {
            this.m_name = methodName;
            this.m_content = CONTENT;
            this.m_methodContent = METHODCONTENT;
            this.m_decorateTypes = DECORATE.Split('|');
        }

        public void SetAuthority(MethodAuthorityType type)
        {
            this.m_authorityType = type;
        }

        public void SetDecorate(MethodDecorateType type)
        {
            this.m_decorateType = type;
        }

        public void SetMethodName(string name)
        {
            this.m_name = name;
        }

        public void SetReturn(string returnType)
        {
            this.m_returnType = returnType;
        }

        public void SetParms(string[] parms)
        {
            this.m_parms = parms;
        }

        public override string ToString()
        {
            this.m_content = this.m_content.Replace("#Authority#", GetAuthority(this.m_authorityType));
            this.m_content = this.m_content.Replace("#DecorateType#", GetDecorate(this.m_decorateType));
            this.m_content = this.m_content.Replace("#ReturnType#", GetReturnType(this.m_returnType));
            this.m_content = this.m_content.Replace("#MethodContent#", GetMethodContent(this.m_decorateType, this.m_returnType));
            this.m_content = this.m_content.Replace("#MethodName#", this.m_name);
            this.m_content = this.m_content.Replace("#Parms#", GetParms(this.m_parms));
            return this.m_content;
        }

        string GetReturn(string type)
        {
            string intValue = "Int32|int|double|Double|float|Single|";
            if (type.Equals("Boolean") || type.Equals("bool")) return "true";
            string[] intValues = intValue.Split('|');
            for (int i = 0; i < intValues.Length; i++)
            {
                if(type.Equals(intValues[i])) return "0";
            }
            return "null";
        }

        string GetAuthority(MethodAuthorityType type)
        {
            string authorityValue = "";
            if (type == MethodAuthorityType.Default)
            {
                authorityValue = ClassGenerator.authorityTypes[(int)type];
            }
            else
            {
                authorityValue = ClassGenerator.authorityTypes[(int)type] + " ";
            }
            return authorityValue;
        }

        string GetDecorate(MethodDecorateType type)
        {
            string decorateValue = "";
            if (type == MethodDecorateType.Default)
            {
                decorateValue = this.m_decorateTypes[(int)type];
            }
            else
            {
                decorateValue = this.m_decorateTypes[(int)type] + " ";
            }
            return decorateValue;
        }

        string GetParms(string[] parms)
        {
            string parmsValue = "";
            if (parms != null && parms.Length > 0)
            {
                for (int i = 0; i < parms.Length; i++)
                {
                    parmsValue += string.Format("{0} {1}Value,", parms[i], parms[i]);
                }
                parmsValue = parmsValue.Substring(0, parmsValue.Length - 1);
            }
            return parmsValue;
        }

        string GetReturnType(string returnType)
        {
            if (returnType == null)
            {
                returnType = "void";
            }
            return returnType;
        }

        string GetMethodContent(MethodDecorateType type,string returnType)
        {
            if (type == MethodDecorateType.Abstract)
            {
                this.m_methodContent = ";";
            }
            else
            {
                string returnValue = null;
                if (returnType != null)
                {
                    returnValue = string.Format("return {0};", GetReturn(returnType));
                }
                else
                {
                    returnValue = "";
                }
                this.m_methodContent = this.m_methodContent.Replace("#ReturnValue#", returnValue);
            }
            return this.m_methodContent;
        }
    }

    public class PropertyGenerator : IPropertyGenerator
    {
        private string m_name;

        public void SetAuthority(AuthorityType type)
        {
            
        }

        public void SetPropertyName(string name)
        {
            
        }

        public override string ToString()
        {
            return null;
        }
    }

    public class FieldGenerator : IFieldGenerator
    {
        private const string CONTENT = @"    #Authority##ValueType# #Name#; #Annotation#";

        private string m_content;

        private AuthorityType m_authorityType = AuthorityType.Private;

        private string m_name;

        private string m_valueType;

        private string m_annotation;

        public FieldGenerator()
        {
            this.m_content = CONTENT;
        }

        public void SetAuthority(AuthorityType type)
        {
            this.m_authorityType = type;
        }

        public void SetFieldName(string name)
        {
            this.m_name = name;
        }

        public void SetFieldType(string type)
        {
            this.m_valueType = type;
        }

        public void SetAnnotation(string annotation)
        {
            this.m_annotation = annotation;
        }

        public override string ToString()
        {
            this.m_content = this.m_content.Replace("#Authority#", GetAuthority());
            this.m_content = this.m_content.Replace("#ValueType#", this.m_valueType);
            this.m_content = this.m_content.Replace("#Name#", this.m_name);
            this.m_content = this.m_content.Replace("#Annotation#", GetAnnotation());
            return this.m_content;
        }

        string GetAuthority()
        {
            string authorityValue = "";
            if (m_authorityType == AuthorityType.Default)
            {
                authorityValue = ClassGenerator.authorityTypes[(int)m_authorityType];
            }
            else
            {
                authorityValue = ClassGenerator.authorityTypes[(int)m_authorityType] + " ";
            }
            return authorityValue;
        }

        string GetAnnotation()
        {
            if (string.IsNullOrEmpty(this.m_annotation)) return "";
            return "// " + this.m_annotation;
        }
    }
}

