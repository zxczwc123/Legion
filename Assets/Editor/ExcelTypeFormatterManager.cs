using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public delegate object DelegateTypeParseFormatter(string value);

public delegate string DelegateTypeStringFormatter(object value);

public class ExcelTypeFormatterManager
{
    private static Dictionary<Type, DelegateTypeParseFormatter> parseFormatterDict;
    
    private static Dictionary<Type, DelegateTypeStringFormatter> stringFormatterDict;

    public static DelegateTypeParseFormatter GetParseFormatter(Type type)
    {
        if (parseFormatterDict == null)
        {
            InitParseFormatter();
        }
        if (parseFormatterDict.ContainsKey(type))
        {
            return parseFormatterDict[type];
        }
        return null;
    }
    
    public static DelegateTypeStringFormatter GetStringFormatter(Type type)
    {
        if (stringFormatterDict == null)
        {
            InitStringFormatter();
        }
        if (stringFormatterDict.ContainsKey(type))
        {
            return stringFormatterDict[type];
        }
        return null;
    }

    public static void InitParseFormatter()
    {
        parseFormatterDict = new Dictionary<Type, DelegateTypeParseFormatter>();
        parseFormatterDict.Add(typeof(int),ParseInt);
        
        parseFormatterDict.Add(typeof(float),ParseFloat);
        parseFormatterDict.Add(typeof(double),ParseDouble);
        parseFormatterDict.Add(typeof(int[]),ParseIntArray);
        parseFormatterDict.Add(typeof(Vector3),ParseVector3);
    }
    
    public static void InitStringFormatter()
    {
        stringFormatterDict = new Dictionary<Type, DelegateTypeStringFormatter>();
        stringFormatterDict.Add(typeof(int[]),FormatIntArray);
    }

    public static object ParseInt(string value)
    {
        return int.Parse(value);
    }
    
    public static object ParseIntArray(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }
        var array = value.Split(',');
        var arr = Array.ConvertAll(array, (x) => int.Parse(x));
        return arr;
    }

    public static string FormatIntArray(object value)
    {
        var intArr = value as int[];
        if (intArr == null || intArr.Length == 0)
        {
            return null;
        }
        var builder = new StringBuilder();
        for (var i = 0; i < intArr.Length; i++)
        {
            builder.Append(intArr[i]);
            if (i != intArr.Length - 1)
            {
                builder.Append(',');
            }
        }
        return builder.ToString();
    }
    
    public static object ParseFloat(string value)
    {
        return float.Parse(value);
    }
    
    public static object ParseDouble(string value)
    {
        return double.Parse(value);
    }

    public static object ParseVector3(string value)
    {
        var array = value.Substring(1, value.Length - 2).Split(',');
        var floatArray = Array.ConvertAll(array, (x) => float.Parse(x));
        return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
    }
}