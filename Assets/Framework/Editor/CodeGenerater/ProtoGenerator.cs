// ========================================================
// 描 述：ProtoGenerator.cs 
// 作 者：郑贤春 
// 时 间：2017/02/21 10:59:20 
// 版 本：5.4.1f1 
// ========================================================
using System;
using System.Collections.Generic;

namespace Framework.MEditor.CodeGenerator
{
    public class ProtoGenerator
    {
        
        public enum PropertyType
        {
            Common = 0,
            List = 1
        }

        public enum FormatType
        {
            Default = 0,
            TwosComplement = 1,
            FixedSize = 2
        }

        private const string CONTENTNAMESPACE =
@"#UsingNames#
namespace #Namespace#
{
#Content#
}";
        private const string CONTENTWITHOUTNAMESPACE =
@"#UsingNames#
#Content#
";
        private const string CONTENT =
@"[global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@""#ClassName#"")]
public partial class #ClassName# : global::ProtoBuf.IExtensible
{
    public #ClassName#() {}
    
#Property#

    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
    { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
}";
        private const string PROPERTY =
@"    private #Type# _#PropertyName#;
    [global::ProtoBuf.ProtoMember(#Number#, IsRequired = true, Name=@""#PropertyName#"", DataFormat = global::ProtoBuf.DataFormat.#FormatType#)]
    public #Type# #PropertyName#
    {
        get { return _#PropertyName#; }
        set { _#PropertyName# = value; }
    }";
        private const string LISTPROPERTY =
@"    private readonly global::System.Collections.Generic.#Type# _#PropertyName# = new global::System.Collections.Generic.#Type#();
    [global::ProtoBuf.ProtoMember(#Number#, Name = @""#PropertyName#"", DataFormat = global::ProtoBuf.DataFormat.#FormatType#)]
    public global::System.Collections.Generic.#Type# #PropertyName#
    {
        get { return _#PropertyName#; }
    }";

        private const string USINGNAME = "using #UsingName#;\r\n";
        private const string FORMATTYPE = @"Default|TwosComplement|FixedSize";
        private const string BASETYPE = @"string|bool|int|float|double";

        private static string[] formatTypes = FORMATTYPE.Split('|');

        private string m_content;
        private string m_name;
        private string m_namespace;
        private List<ProtoProperty> m_properties;
        private List<string> m_usingNames;

        public ProtoGenerator()
        {
            this.m_properties = new List<ProtoProperty>();
            this.m_usingNames = new List<string>();
        }

        public ProtoGenerator(string className)
        {
            this.m_name = className;
            this.m_properties = new List<ProtoProperty>();
            this.m_usingNames = new List<string>();
        }

        public void SetName(string className)
        {
            this.m_name = className;
        }

        public void SetNamespace(string _namespace)
        {
            this.m_namespace = _namespace;
        }

        public void AddUsingName(string name)
        {
            if (this.m_usingNames.Contains(name)) return;
            this.m_usingNames.Add(name);   
        }

        public void AddProperty(string propertyName,Type type)
        {
            AddProperty(propertyName, type.Name);
        }

        public void AddProperty(string propertyName, string type)
        {
            ProtoProperty property = new ProtoProperty() { propertyName = propertyName, type = type};
            this.m_properties.Add(property);
        }

        public override string ToString()
        {
            this.m_content = CONTENT;
            this.m_content = this.m_content.Replace("#ClassName#", this.m_name);
            this.m_content = this.m_content.Replace("#Property#", GetProperties());
            if(!string.IsNullOrEmpty(this.m_namespace))
            {
                this.m_content = CONTENTNAMESPACE.Replace("#Content#", RetractContent(this.m_content));
                this.m_content = this.m_content.Replace("#Namespace#", this.m_namespace);
            }
            else
            {
                this.m_content = CONTENTWITHOUTNAMESPACE.Replace("#Content#", this.m_content);
            }
            this.m_content = this.m_content.Replace("#UsingNames#", GetUsingNames());
            return this.m_content;
        }

        private string GetProperties()
        {
            string result = "";
            if (this.m_properties == null || this.m_properties.Count < 1) return result;
            for(int i=0;i<this.m_properties.Count;i++)
            {
                ProtoProperty property = this.m_properties[i];
                result += property.ToString(i + 1);
                if (i < this.m_properties.Count - 1) result += "\r\n";
            }
            return result;
        }

        private string GetUsingNames()
        {
            string result = "";
            if (this.m_usingNames == null || this.m_usingNames.Count < 1) return result;
            for(int i=0;i<this.m_usingNames.Count;i++)
            {
                result += USINGNAME.Replace("#UsingName#", this.m_usingNames[i]);
            }
            return result;
        }

        private string RetractContent(string content)
        {
            return "    " + content.Replace("\r\n", "\r\n    "); ;
        }

        private struct ProtoProperty
        {
            public string propertyName;
            public string type;

            public string ToString(int index)
            {
                string result = null;
                if(!type.Contains("List"))
                    result = PROPERTY;
                else
                    result = LISTPROPERTY;
                result = result.Replace("#PropertyName#", propertyName);
                result = result.Replace("#Type#", type);
                result = result.Replace("#Number#", index.ToString());
                result = result.Replace("#FormatType#", GetFormatType());
                return result;
            }

            private string GetFormatType()
            {
                int index = 0;
                if("int".Equals(type) || "long".Equals(type))
                {
                    index = (int)FormatType.TwosComplement;
                }
                else if("float".Equals(type) || "double".Equals(type))
                {
                    index = (int)FormatType.FixedSize;
                }
                return formatTypes[index];
            }
        }
    }
}