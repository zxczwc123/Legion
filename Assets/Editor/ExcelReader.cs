using System.Collections.Generic;
using OfficeOpenXml;

public class ExcelReader
{
    public static List<string> ReadRowData(ExcelRange cells, int row , int colCount)
    {
        List<string> rowData = null;
        for (var col = 1; col <= colCount; col++)
        {
            var text = cells[row, col];
            if (col == 1)
            {
                if(string.IsNullOrEmpty(text.Text.Trim()))
                {
                    return null;
                }
                rowData = new List<string>();
            }
            rowData.Add(text.Text);
        }
        return rowData;
    }
}