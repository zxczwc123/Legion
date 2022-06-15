using System;

public static class StringExtension
{
    public static string Extension(this string str)
    {
        var lastIndex = str.LastIndexOf(".", StringComparison.Ordinal);
        if (lastIndex < 0)
        {
            return "";
        }
        return str.Substring(lastIndex);
    }

    public static string NameWithoutExtension(this string str)
    {
        var lastIndex = str.LastIndexOf(".", StringComparison.Ordinal);
        if (lastIndex < 0)
        {
            return str;
        }
        return str.Substring(0, lastIndex);
    }
}