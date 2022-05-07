using System.Collections.Generic;
using System.IO;
using System.Reflection;
using OfficeOpenXml;
using UnityEngine;

public class ExcelDataWriter
{
    private List<string> m_TypeNameList;
    
    private List<string> m_TypeList;
    
    private List<List<string>> m_ValueList;

    private ExcelPackage m_Package;

    private ExcelWorksheet m_Worksheet;
    
    public ExcelDataWriter(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError(string.Format("文件不存在：{0}", path));
            return;
        }
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        m_Package = new ExcelPackage(new FileInfo(path));
        m_Worksheet = m_Package.Workbook.Worksheets[0];
        if (m_Worksheet == null)
        {
            return;
        }
        Read();
    }
    
    private void Read()
    {
        m_TypeList = new List<string>();
        m_ValueList = new List<List<string>>();
        var cells = m_Worksheet.Cells;
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
    
    public void SaveDict<T>(Dictionary<int, T> dict) 
    {
        if (m_ValueList == null)
        {
            return;
        }
        var type = typeof(T);
        var fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        var fieldDict = new Dictionary<string, FieldInfo>();
        foreach (var fieldInfo in fieldInfos)
        {
            fieldDict.Add(fieldInfo.Name, fieldInfo);
        }
        var row = 4;
        foreach (var value in dict.Values)
        {
            SetRow(row,value,fieldDict);
            row++;
        }
        m_Package.Save();
    }

    public void SetRow<T>(int row, T data,Dictionary<string, FieldInfo> fieldDict)
    {
        var cells = m_Worksheet.Cells;
        for (var i=0;i<  m_TypeNameList.Count;i++)
        {
            var col = i + 1;
            var name = m_TypeNameList[i];
            if (fieldDict.ContainsKey(name))
            {
                var fieldInfo = fieldDict[name];
                cells.SetCellValue(row - 1, col - 1, fieldInfo.GetValue(data));
            }
        }
    }
}