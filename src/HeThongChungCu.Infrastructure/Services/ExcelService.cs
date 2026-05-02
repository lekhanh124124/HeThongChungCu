using HeThongChungCu.Application.Common.Interfaces.Services;
using OfficeOpenXml;
using System.ComponentModel;

namespace HeThongChungCu.Infrastructure.Services;

public class ExcelService : IExcelService
{
    public ExcelService()
    {
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
    }

    public byte[] CreateTemplate<T>(IEnumerable<T> data, string sheetName)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add(sheetName);

        var properties = typeof(T).GetProperties();
        
        // Headers
        for (int i = 0; i < properties.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = properties[i].Name;
            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
        }

        // Data
        var row = 2;
        foreach (var item in data)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cells[row, i + 1].Value = properties[i].GetValue(item);
            }
            row++;
        }

        // Add extra columns for input if it's a template
        // For ChiSoExcelTemplateDto, we need "SoMoi" and "GhiChu"
        if (typeof(T).Name.Contains("Template"))
        {
            worksheet.Cells[1, properties.Length + 1].Value = "SoMoi";
            worksheet.Cells[1, properties.Length + 1].Style.Font.Bold = true;
            worksheet.Cells[1, properties.Length + 2].Value = "GhiChu";
            worksheet.Cells[1, properties.Length + 2].Style.Font.Bold = true;
        }

        worksheet.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }

    public List<T> Import<T>(Stream stream) where T : new()
    {
        var list = new List<T>();
        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets[0];
        var rowCount = worksheet.Dimension.Rows;
        var colCount = worksheet.Dimension.Columns;

        var properties = typeof(T).GetProperties();
        var headerMap = new Dictionary<int, string>();

        for (int col = 1; col <= colCount; col++)
        {
            headerMap[col] = worksheet.Cells[1, col].Text;
        }

        for (int row = 2; row <= rowCount; row++)
        {
            var item = new T();
            foreach (var prop in properties)
            {
                var colIndex = headerMap.FirstOrDefault(x => x.Value.Equals(prop.Name, StringComparison.OrdinalIgnoreCase)).Key;
                if (colIndex > 0)
                {
                    var cellValue = worksheet.Cells[row, colIndex].Value;
                    if (cellValue != null)
                    {
                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        prop.SetValue(item, Convert.ChangeType(cellValue, targetType));
                    }
                }
            }
            list.Add(item);
        }

        return list;
    }
}
