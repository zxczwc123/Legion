using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OfficeOpenXml;
using PlasticGui.WorkspaceWindow.Merge;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class ExcelDataReader
{
    private List<string> m_TypeNameList;
    
    private List<string> m_TypeList;
    
    private List<List<string>> m_ValueList;
    
    public ExcelDataReader(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError(string.Format("文件不存在：{0}", path));
            return;
        }
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var package = new ExcelPackage(new FileInfo(path));
        var worksheet = package.Workbook.Worksheets[0];
        if (worksheet == null)
        {
            return;
        }
        Read(worksheet);
    }
    
    public ExcelDataReader(Stream stream)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets[0];
        if (worksheet == null)
        {
            return;
        }
        Read(worksheet);
    }

    private void Read(ExcelWorksheet worksheet)
    {
        m_TypeList = new List<string>();
        m_ValueList = new List<List<string>>();
        var cells = worksheet.Cells;
        var rows = cells.Rows;
        ReadTypeNames(cells, 2);
        var colCount = m_TypeNameList.Count;
        m_TypeList = ExcelReader.ReadRowData(cells, 3, colCount);
        for (int i = 4; i <= rows; i++)
        {
            var rowData = ExcelReader.ReadRowData(cells, i, colCount);
            if (rowData == null)
                break;
            m_ValueList.Add(rowData);
        }
    }

    private void ReadTypeNames(ExcelRange cells, int row)
    {
        var col = 1;
        var text = cells[row, col];
        m_TypeNameList = new List<string>(); 
        while (!string.IsNullOrEmpty(text.Text.Trim()))
        {
            m_TypeNameList.Add(text.Text);
            col++;
            text =  cells[row, col];
        }
    }
    
    

    public Dictionary<int, T> GetDict<T>() 
    {
        if (m_ValueList == null || m_ValueList.Count == 0)
        {
            return default;
        }
        var type = typeof(T);
        var fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        var fieldDict = new Dictionary<string, FieldInfo>();
        foreach (var fieldInfo in fieldInfos)
        {
            fieldDict.Add(fieldInfo.Name, fieldInfo);
        }
        var resultDict = new Dictionary<int,T>();
        for (var i = 0; i < m_ValueList.Count; i++)
        {
            var rowData = m_ValueList[i];
            var data = GetRow<T>(rowData, fieldDict, out int key);
            if (!resultDict.ContainsKey(key))
            {
                resultDict.Add(key, data);
            }
            else
            {
                Debug.LogError($"key is duplicate value is {key} row is {i + 3}");
            }
        }
        return resultDict;
    }

    private T GetRow<T>(List<string> rowData, Dictionary<string, FieldInfo> fieldDict,out int key)
    {
        var data = Activator.CreateInstance<T>();
        key = 0;
        for (var i = 0; i < rowData.Count; i++)
        {
            var typeName = m_TypeNameList[i];
            var typeValueStr = rowData[i];
            if (!fieldDict.ContainsKey(typeName))
            {
                continue;
            }
            var fieldInfo = fieldDict[typeName];
            var value = FormatValue(fieldInfo.FieldType, typeValueStr);
            fieldInfo.SetValue(data, value);
            if (i == 0)
            {
                if (value is int)
                {
                    key = (int)value;
                }
                else
                {
                    Debug.LogError($"key is not int where value is {value}");
                }
            }
        }
        return data;
    }

    private object FormatValue(Type type, string value)
    {
        var parseFormatter = ExcelTypeFormatterManager.GetParseFormatter(type);
        if (parseFormatter != null)
        {
            return parseFormatter(value);
        }
        return value;
    }
}
