// ========================================================
// 描 述：ExcelUtility.cs 
// 作 者：郑贤春 
// 时 间：2017/01/07 09:13:19 
// 版 本：5.4.1f1 
// ========================================================
using System.IO;
using Excel;
using System.Collections.Generic;
using System;
using System.Reflection;
using Framework.MEditor.CodeGenerator;
using UnityEngine;
using System.Data;

namespace Framework.MEditor.Utility
{
    public struct ClassData
    {
        public string className;
        public string classContent;
    }

    public interface ExcelFilter
    {
        string GetReturnType();
        string[] GetUsings();
        object GetValue(string value);
    }

    public class Vector3Filter : ExcelFilter
    {
        public string GetReturnType()
        {
            return "Vector3";
        }

        public string[] GetUsings()
        {
            return new string[] { "UnityEngine" };
        }

        public object GetValue(string value)
        {
            string[] values = value.Split(',');
            return new Vector3(float.Parse(values[0]), float.Parse(values[0]), float.Parse(values[0]));
        }
    }

    public class ListIntFilter : ExcelFilter
    {
        public string GetReturnType()
        {
            return "List<int>";
        }

        public string[] GetUsings()
        {
            return new string[] { "System.Collections.Generic" };
        }

        public object GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            string[] values = value.Split(',');
            List<int> result = new List<int>();
            for(int i=0;i<values.Length;i++)
            {
                result.Add(int.Parse(values[i]));
            }
            return result;
        }
    }

    public class ExcelUtility
    {
        private const string ASSEMBLYNAME = "Assembly-CSharp";

        public static Dictionary<int,T> ToFieldObjectDict<T>(string path)
        {
            Type type = typeof(T);
            SheetData sheet = GetSheet(path, type.Name);
            if (sheet == null) return null;
            FieldInfo[] fields = type.GetFields();
            Dictionary<int, T> dict = new Dictionary<int, T>();
            for(int index= 0;index < sheet.ids.Length;index++)
            {
                string id = sheet.ids[index];
                T t = Activator.CreateInstance<T>();
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo field = fields[i];
                    Type fieldType = field.FieldType;
                    field.SetValue(t, sheet.GetValue(id, field.Name, fieldType));
                }
                dict.Add(int.Parse(id), t);
            }
            return dict;
        }

        public static object ToFieldObjectDict(string path,int sheetIndex = 0)
        {
            SheetData sheet = GetSheet(path, sheetIndex);
            if (sheet == null) return null;
            Assembly assembly = Assembly.Load(ASSEMBLYNAME);
            Type type = assembly.GetType(sheet.sheetName);
            if (type == null) return null;
            FieldInfo[] fields = type.GetFields();
            Type dictType = typeof(Dictionary<,>).MakeGenericType(new Type[] { typeof(int), type });
            object dict = Activator.CreateInstance(dictType);
            MethodInfo addMethod = dictType.GetMethod("Add");
            for (int index = 0; index < sheet.ids.Length; index++)
            {
                string id = sheet.ids[index];
                object obj = Activator.CreateInstance(type);
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo field = fields[i];
                    Type fieldType = field.FieldType;
                    field.SetValue(obj, sheet.GetValue(id, field.Name, fieldType));
                }
                addMethod.Invoke(dict, new object[] { int.Parse(id), obj });
            }
            return dict;
        }

        public static void ToFieldClass(string path)
        {
            ExcelData data = GetData(path);
            string directory = Path.GetDirectoryName(path);
            for(int i=0;i<data.sheetDatas.Count;i++)
            {
                SheetData sheet = data.sheetDatas[i];
                ClassData classData = sheet.ToFieldClass();
                string filePath = directory + "/" + classData.className + ".cs";
                StringToFile(filePath, classData.classContent);
            }
        }

        public static void ToProtoClass(string path)
        {
            ExcelData data = GetData(path);
            string directory = Path.GetDirectoryName(path);
            for (int i = 0; i < data.sheetDatas.Count; i++)
            {
                SheetData sheet = data.sheetDatas[i];
                ClassData classData = sheet.ToProtoClass();
                string filePath = directory + "/" + classData.className + ".cs";
                StringToFile(filePath, classData.classContent);
            }
        }

        public static object ToProtoObjectDict(string path, int sheetIndex = 0)
        {
            SheetData sheet = GetSheet(path, sheetIndex);
            if (sheet == null) return null;
            Assembly assembly = Assembly.Load(ASSEMBLYNAME);
            Type type = assembly.GetType(sheet.sheetName);
            if (type == null) return null;
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            Type dictType = typeof(Dictionary<,>).MakeGenericType(new Type[] { typeof(int), type});
            object dict = Activator.CreateInstance(dictType);
            MethodInfo addMethod = dictType.GetMethod("Add");
            for (int index = 0; index < sheet.ids.Length; index++)
            {
                string id = sheet.ids[index];
                object obj = Activator.CreateInstance(type);
                for (int i = 0; i < fieldInfos.Length; i++)
                {
                    FieldInfo field = fieldInfos[i];
                    Type fieldType = field.FieldType;
                    string fieldName = field.Name.Substring(1);
                    field.SetValue(obj, sheet.GetValue(id, fieldName, fieldType));
                }
                addMethod.Invoke(dict, new object[] { int.Parse(id), obj });
            }
            return dict;
        }

        public static ClassData[] GetFieldClass(string path)
        {
            ExcelData data = GetData(path);
            string directory = Path.GetDirectoryName(path);
            int count = data.sheetDatas.Count;
            if (count == 0) return null;
            ClassData[] result = new ClassData[count];
            for (int i = 0; i < count; i++)
            {
                SheetData sheet = data.sheetDatas[i];
                result[i] = sheet.ToFieldClass();
            }
            return result;
        }

        private static ExcelData GetData(string path)
        {
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet dataSet = excelReader.AsDataSet();
            ExcelData result = new ExcelData(dataSet);
            return result;
        }

        private static SheetData GetSheet(string path, int sheetIndex)
        {
            ExcelData data = GetData(path);
            if (data == null || data.sheetDatas == null || data.sheetDatas.Count < 1) return null;
            if (sheetIndex >= data.sheetDatas.Count) return null;
            return data.sheetDatas[sheetIndex];
        }

        private static SheetData GetSheet(string path, string sheetName)
        {
            ExcelData data = GetData(path);
            for (int i=0;i<data.sheetDatas.Count;i++)
            {
                SheetData sheet = data.sheetDatas[i];
                if(sheet.sheetName.Equals(sheetName))
                {
                    return sheet;
                }
            }
            return null;
        }

        private static void StringToFile(string path,string content)
        {
            if(File.Exists(path)) File.Delete(path);
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);
                fileStream.Write(bytes,0,bytes.Length);
            }
        }
    }

    public class SheetData
    {
        public string sheetName;

        public string[] ids;
        public string[] annotations;
        public string[] fieldTypes;
        public string[] memberNames;

        private int m_annotationRow = 0; // 字段说明注释行
        private int m_fieldNameRow = 2; // 字段名所在行
        private int m_fieldTypeRow = 3; // 字段类型所在行
        private int m_contentRow = 4; // 内容起始行

        private Dictionary<string, Dictionary<string, string>> m_data = new Dictionary<string, Dictionary<string, string>>(); // key1 ： id ,key2 : fieldName;

        public SheetData(DataTable table)
        {
            this.sheetName = table.TableName;

            int rowCount = table.Rows.Count;
            int colCount = table.Columns.Count;
            this.annotations = new string[colCount];
            this.memberNames = new string[colCount];
            this.fieldTypes = new string[colCount];
            this.ids = new string[rowCount - m_contentRow];
            for (int row = 0; row < rowCount; row++)
            {
                Dictionary<string, string> rowData = new Dictionary<string, string>();
                for (int col = 0; col < colCount; col++)
                {
                    string value = table.Rows[row][col].ToString();
                    if (row == m_annotationRow) this.annotations[col] = value;
                    if (row == m_fieldNameRow) this.memberNames[col] = value;
                    if (row == m_fieldTypeRow) this.fieldTypes[col] = value;
                    if (col == 0 && row >= this.m_contentRow)
                    {
                        ids[row - this.m_contentRow] = value;
                    }
                    if (row >= this.m_contentRow)
                    {
                        rowData.Add(memberNames[col], value);
                    }
                }
                if (row >= this.m_contentRow)
                {
                    m_data.Add(ids[row - this.m_contentRow], rowData);
                }
            }
        }

        public ClassData ToFieldClass()
        {
            ClassData data = new ClassData();
            data.className = sheetName;
            ClassGenerator generator = new ClassGenerator(sheetName);
            
            generator.SetAuthority(AuthorityType.Public);
            for (int i = 0; i < memberNames.Length; i++)
            {
                FieldGenerator field = new FieldGenerator();
                field.SetAnnotation(annotations[i]);
                field.SetFieldName(memberNames[i]);
                field.SetFieldType(fieldTypes[i]);
                field.SetAuthority(AuthorityType.Public);
                generator.AddField(field);
                Type filterType = GetFilterType(fieldTypes[i]);
                if(filterType != null)
                {
                    MethodInfo usingMethod = filterType.GetMethod("GetUsings");
                    if (usingMethod == null) continue;
                    object filterObj = Activator.CreateInstance(filterType);
                    string[] usingNames = usingMethod.Invoke(filterObj, null) as string[];
                    if (usingNames == null) continue;
                    generator.SetUsingName(usingNames);
                }
            }
            data.classContent = generator.ToString();
            return data;
        }

        public ClassData ToProtoClass()
        {
            ClassData data = new ClassData();
            data.className = sheetName;
            ProtoGenerator generator = new ProtoGenerator(sheetName);
            for (int i = 0; i < memberNames.Length; i++)
            {
                Type filterType = GetFilterType(fieldTypes[i]);
                generator.AddProperty(memberNames[i], fieldTypes[i]);
                if (filterType != null)
                {
                    MethodInfo usingMethod = filterType.GetMethod("GetUsings");
                    if (usingMethod == null) continue;
                    object filterObj = Activator.CreateInstance(filterType);
                    string[] usingNames = usingMethod.Invoke(filterObj, null) as string[];
                    if (usingNames == null) continue;
                    foreach(string name in usingNames)
                    {
                        generator.AddUsingName(name);
                    }
                }
            }
            data.classContent = generator.ToString();
            return data;
        }

        public List<string> GetKeys()
        {
            return new List<string>(m_data.Keys);
        }

        public int GetCount()
        {
            return m_data.Count;
        }

        public object GetValue(string id, string field, Type type)
        {
            if (type.Equals(typeof(string)))
            {
                return GetString(id, field);
            }
            else if (type.Equals(typeof(int)))
            {
                return GetInt(id, field);
            }
            else if (type.Equals(typeof(float)))
            {
                return GetFloat(id, field);
            }
            else if (type.Equals(typeof(double)))
            {
                return GetDouble(id, field);
            }
            else
            {
                Type filterType = GetFilterType(type);
                if (filterType == null) return null;
                MethodInfo valueMethod = filterType.GetMethod("GetValue");
                if (valueMethod == null) return null;
                object filterObj = Activator.CreateInstance(filterType);
                return valueMethod.Invoke(filterObj, new object[] { GetString(id, field) });
            }
        }

        public string GetString(string id, string field)
        {
            return m_data[id][field];
        }

        public int GetInt(string id, string field)
        {
            return int.Parse(m_data[id][field]);
        }

        public float GetFloat(string id, string field)
        {
            return float.Parse(m_data[id][field]);
        }

        public double GetDouble(string id, string field)
        {
            return double.Parse(m_data[id][field]);
        }

        private Type GetFilterType(Type type)
        {
            string typeName = type.Name;
            return GetFilterType(typeName);
        }

        private Type GetFilterType(string type)
        {
            List<Type> filterTypes = GetFilterTypes();
            Type filterType = filterTypes.Find(x => {
                MethodInfo returnMethord = x.GetMethod("GetReturnType");
                if (returnMethord == null) return false;
                object obj = Activator.CreateInstance(x);
                return returnMethord.Invoke(obj, null).ToString() == type;
            });
            return filterType;
        }

        private List<Type> GetFilterTypes()
        {
            Assembly assembly = this.GetType().Assembly;
            List<Type> result = new List<Type>();
            foreach(Type type in assembly.GetTypes())
            {
                if (type.GetInterface(typeof(ExcelFilter).Name) != null) result.Add(type);
            }
            return result;
        }
    }

    public class ExcelData
    {
        public List<SheetData> sheetDatas = new List<SheetData>();

        private int m_desRowCount = 4;// 描述行数
		private int m_sheetCount;
		private int m_validSheetCount;

        public ExcelData(DataSet dataSet)
        {
			this.m_sheetCount = dataSet.Tables.Count;
			for (int i = 0; i < this.m_sheetCount; i++) 
			{
				DataTable dataTable = dataSet.Tables[i];
				if (!IsValidTable (dataTable))
					continue;
				this.m_validSheetCount += 1;
				this.sheetDatas.Add (new SheetData (dataTable));
			}
        }

		bool IsValidTable(DataTable table)
		{
			int row = table.Rows.Count;
			if (row < this.m_desRowCount)
				return false;
			return true;
		}
    }
    
}
