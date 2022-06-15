using Framework.MEditor.Utility;
using UnityEditor;

namespace Framework.MEditor.CodeGenerator {
    public class Generator {
        public static void GenerateCode1 () {
            MethodGenerator method = new MethodGenerator ("Test");
            method.SetAuthority (MethodAuthorityType.Internal);
            method.SetDecorate (MethodDecorateType.Virtual);
            method.SetParms (new string[] { "int", "string" });
            method.SetReturn ("bool");
            MethodGenerator method1 = new MethodGenerator ("Test1");
            method1.SetAuthority (MethodAuthorityType.Internal);
            method1.SetDecorate (MethodDecorateType.Virtual);
            method1.SetParms (new string[] { "int", "string" });
            method1.SetReturn ("bool");
            ClassGenerator textClass = new ClassGenerator ("TestClass");
            textClass.SetUsingName (new string[] { "xxxx", "aaaa" });
            textClass.SetBaseClass ("xxcvsdf");
            textClass.SetDecorate (ClassDecorateType.Abstract);
            textClass.SetAuthority (AuthorityType.Public);
            textClass.SetInterfaces (new string[] { "asdfsadf", "asdfasdf" });
            textClass.SetNamespace ("masdjf");
            textClass.AddMethod (method);
            textClass.AddMethod (method1);
            string classValue = textClass.ToString ();
            TxtUtility.StringToFile (classValue);
        }

        public static void GenerateCode2 () {
            FieldGenerator field = new FieldGenerator ();
            field.SetAuthority (AuthorityType.Private);
            field.SetFieldName ("m_xxxx");
            field.SetFieldType ("bool");
            FieldGenerator field1 = new FieldGenerator ();
            field1.SetAuthority (AuthorityType.Private);
            field1.SetFieldName ("m_xxxx");
            field1.SetFieldType ("bool");
            MethodGenerator method = new MethodGenerator ("Test");
            method.SetAuthority (MethodAuthorityType.Internal);
            method.SetDecorate (MethodDecorateType.Virtual);
            method.SetParms (new string[] { "int", "string" });
            method.SetReturn ("bool");
            MethodGenerator method1 = new MethodGenerator ("Test1");
            method1.SetAuthority (MethodAuthorityType.Internal);
            method1.SetDecorate (MethodDecorateType.Virtual);
            method1.SetParms (new string[] { "int", "string" });
            method1.SetReturn ("bool");
            ClassGenerator textClass = new ClassGenerator ("TestClass");
            //textClass.SetUsingName(new string[] { "xxxx", "aaaa" });
            //textClass.SetNamespace("masdjf");
            textClass.SetBaseClass ("xxcvsdf");
            textClass.SetDecorate (ClassDecorateType.Abstract);
            textClass.SetAuthority (AuthorityType.Public);
            textClass.SetInterfaces (new string[] { "asdfsadf", "asdfasdf" });
            textClass.AddMethod (method);
            textClass.AddMethod (method1);
            textClass.AddField (field);
            textClass.AddField (field1);
            string classValue = textClass.ToString ();
            TxtUtility.StringToFile (classValue);
            //Debug.Log(classValue);
        }

        public static void GenerateCode3 () {
            var textClass = new ProtoGenerator ("TestClass");
            textClass.SetNamespace ("MM");
            textClass.AddProperty ("id", "int");
            textClass.AddProperty ("id1", "int");
            string classValue = textClass.ToString ();
            TxtUtility.StringToFile (classValue);
            //Debug.Log(classValue);
        }
    }
}