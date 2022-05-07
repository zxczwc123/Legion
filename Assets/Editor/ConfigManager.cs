using System.Collections.Generic;
using System.Net;
using DefaultNamespace.Data;
using ICSharpCode.NRefactory.Ast;
using UnityEngine;

public class ConfigManager
{
    private static Dictionary<string, object> configDict = new Dictionary<string, object>();
    
    private static string GetConfigExcelPath(string configName)
    {
        return Application.dataPath + $"/Editor/Config/{configName}.xlsx";
    }
    
    public static Dictionary<int, T> GetDict<T>()
    {
        var configName = typeof(T).Name;
        var path = GetConfigExcelPath(configName);
        if (configDict.ContainsKey(path))
        {
            return (Dictionary<int, T>)configDict[path];
        }
        var excelReader = new ExcelDataReader(path);
        var dict = excelReader.GetDict<T>();
        configDict.Add(path,dict);
        return dict;
    }

    public static void SaveDict<T>(Dictionary<int, T> config)
    {
        var configName = typeof(T).Name;
        var path = GetConfigExcelPath(configName);
        var excelReader = new ExcelDataWriter(path);
        excelReader.SaveDict(config);
    }

    public static void Release()
    {
        configDict.Clear();
    }
}