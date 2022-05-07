using System;
using System.Collections.Generic;
using UnityEngine;

public class ExcelTypeFormatterManager
{
    private static Dictionary<Type, DelegateTypeParseFormatter> formatterDict;

    public static DelegateTypeParseFormatter GetParseFormatter(Type type)
    {
        if (formatterDict == null)
        {
            InitFormatter();
        }
        if (formatterDict.ContainsKey(type))
        {
            return formatterDict[type];
        }
        return null;
    }

    public static void InitFormatter()
    {
        formatterDict = new Dictionary<Type, DelegateTypeParseFormatter>();
        formatterDict.Add(typeof(int),FormatInt);
        
        formatterDict.Add(typeof(float),FormatFloat);
        formatterDict.Add(typeof(double),FormatDouble);
        formatterDict.Add(typeof(int[]),FormatIntArray);
        formatterDict.Add(typeof(Vector3),FormatVector3);
    }

    public static object FormatInt(string value)
    {
        return int.Parse(value);
    }
    
    public static object FormatIntArray(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }
        return null;
    }
    
    public static object FormatFloat(string value)
    {
        return float.Parse(value);
    }
    
    public static object FormatDouble(string value)
    {
        return double.Parse(value);
    }

    public static object FormatVector3(string value)
    {
        var array = value.Substring(1, value.Length - 2).Split(',');
        var floatArray = Array.ConvertAll(array, (x) => float.Parse(x));
        return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
    }
}