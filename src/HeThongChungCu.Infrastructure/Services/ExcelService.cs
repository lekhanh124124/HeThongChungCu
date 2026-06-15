using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.ComponentModel;
using System.Drawing;

namespace HeThongChungCu.Infrastructure.Services;

public class ExcelService : IExcelService
{
    public ExcelService()
    {
        ExcelPackage.License.SetNonCommercialPersonal("HeThongChungCu");
    }

    public byte[] CreateTemplate<T>(IEnumerable<T> data, string sheetName)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add(sheetName);

        // Detect actual type if T is object (common when using anonymous types in List<object>)
        Type itemType = typeof(T);
        if (itemType == typeof(object) && data != null && data.Any())
        {
            itemType = data.First()!.GetType();
        }

        var properties = itemType.GetProperties();
        
        // Headers
        for (int i = 0; i < properties.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = properties[i].Name.Replace("_", " ");
            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
        }

        // Data
        var row = 2;
        foreach (var item in data)
        {
            if (item == null) continue;
            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cells[row, i + 1].Value = properties[i].GetValue(item);
            }
            row++;
        }

        // Add extra columns for input if it's a template
        // For ChiSoExcelTemplateDto, we need "SoMoi" and "GhiChu"
        if (itemType.Name.Contains("Template"))
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

    public byte[] ExportPhieuBaoTri(PhieuBaoTriDetailResponse phieu)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("PhieuBaoTri");

        // Page settings for A4 portrait print
        worksheet.PrinterSettings.PaperSize = ePaperSize.A4;
        worksheet.PrinterSettings.Orientation = eOrientation.Portrait;
        worksheet.PrinterSettings.FitToWidth = 1;
        worksheet.PrinterSettings.FitToHeight = 0; // flexible height
        worksheet.PrinterSettings.TopMargin = 0.5;
        worksheet.PrinterSettings.BottomMargin = 0.5;
        worksheet.PrinterSettings.LeftMargin = 0.5;
        worksheet.PrinterSettings.RightMargin = 0.5;

        // Set default font
        worksheet.Cells.Style.Font.Name = "Segoe UI";
        worksheet.Cells.Style.Font.Size = 10;

        // Title Block
        var titleCell = worksheet.Cells["A2:E2"];
        titleCell.Merge = true;
        titleCell.Value = "PHIẾU YÊU CẦU & BIÊN BẢN BẢO TRÌ THIẾT BỊ";
        titleCell.Style.Font.Size = 16;
        titleCell.Style.Font.Bold = true;
        titleCell.Style.Font.Color.SetColor(Color.FromArgb(31, 78, 120)); // Navy blue
        titleCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        var subtitleCell = worksheet.Cells["A3:E3"];
        subtitleCell.Merge = true;
        subtitleCell.Value = $"Mã số phiếu: {phieu.MaPhieu}   |   Trạng thái: {phieu.TenTrangThaiPhieuBaoTri}";
        subtitleCell.Style.Font.Size = 11;
        subtitleCell.Style.Font.Bold = true;
        subtitleCell.Style.Font.Italic = true;
        subtitleCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        // Divider
        worksheet.Cells["A4:E4"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
        worksheet.Cells["A4:E4"].Style.Border.Bottom.Color.SetColor(Color.FromArgb(31, 78, 120));

        // Metadata Block
        // Row 5
        var cellA5 = worksheet.Cells["A5:E5"];
        cellA5.Merge = true;
        cellA5.RichText.Add("Thiết bị bảo trì: ").Bold = true;
        cellA5.RichText.Add($"{phieu.TenThietBi} ({phieu.MaThietBi})").Bold = false;

        // Row 6
        var cellA6 = worksheet.Cells["A6:E6"];
        cellA6.Merge = true;
        cellA6.RichText.Add("Hạng mục bảo trì: ").Bold = true;
        cellA6.RichText.Add(phieu.TenHangMuc).Bold = false;

        // Row 7
        var cellA7 = worksheet.Cells["A7:E7"];
        cellA7.Merge = true;
        cellA7.RichText.Add("Nhân sự phân công: ").Bold = true;
        var staffText = phieu.NhanSuBaoTris.Any() 
            ? string.Join(", ", phieu.NhanSuBaoTris.Select(n => $"{n.HoTen} ({n.VaiTro ?? "Kỹ thuật viên"})")) 
            : (phieu.TenDoiTac ?? "Chưa phân công");
        cellA7.RichText.Add(staffText).Bold = false;
        cellA7.Style.WrapText = true;

        // Row 8
        var cellA8 = worksheet.Cells["A8:E8"];
        cellA8.Merge = true;
        cellA8.RichText.Add("Vị trí lắp đặt: ").Bold = true;
        cellA8.RichText.Add("Khu vực kỹ thuật tòa nhà").Bold = false;

        // Row 9
        var cellA9 = worksheet.Cells["A9:E9"];
        cellA9.Merge = true;
        cellA9.RichText.Add("Hợp đồng thầu: ").Bold = true;
        cellA9.RichText.Add(phieu.SoHopDong ?? "Nội bộ").Bold = false;

        // Row 10
        var cellA10 = worksheet.Cells["A10:C10"];
        cellA10.Merge = true;
        cellA10.RichText.Add("Ngày lập: ").Bold = true;
        cellA10.RichText.Add(phieu.NgayLapPhieu.ToString("dd/MM/yyyy HH:mm")).Bold = false;

        var cellD10 = worksheet.Cells["D10:E10"];
        cellD10.Merge = true;
        cellD10.RichText.Add("Ngày dự kiến: ").Bold = true;
        cellD10.RichText.Add(phieu.NgayDuKien.ToString("dd/MM/yyyy")).Bold = false;

        // Row 11
        var cellA11 = worksheet.Cells["A11:C11"];
        cellA11.Merge = true;
        cellA11.RichText.Add("Ngày nghiệm thu: ").Bold = true;
        cellA11.RichText.Add(phieu.NgayThucTe?.ToString("dd/MM/yyyy") ?? "....................").Bold = false;

        var cellD11 = worksheet.Cells["D11:E11"];
        cellD11.Merge = true;
        cellD11.RichText.Add("Trạng thái: ").Bold = true;
        cellD11.RichText.Add(phieu.TenTrangThaiPhieuBaoTri).Bold = false;

        // Divider
        worksheet.Cells["A12:E12"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        worksheet.Cells["A12:E12"].Style.Border.Bottom.Color.SetColor(Color.LightGray);

        // Section Title: Checklist
        var secTitleChecklist = worksheet.Cells["A13:E13"];
        secTitleChecklist.Merge = true;
        secTitleChecklist.Value = "I. DANH SÁCH HẠNG MỤC KIỂM TRA (CHECKLIST)";
        secTitleChecklist.Style.Font.Bold = true;
        secTitleChecklist.Style.Font.Size = 11;
        secTitleChecklist.Style.Font.Color.SetColor(Color.FromArgb(31, 78, 120));

        // Checklist Table Header
        int curRow = 14;
        worksheet.Cells[curRow, 1].Value = "STT";
        worksheet.Cells[curRow, 2].Value = "Nội dung kiểm tra tiêu chuẩn";
        worksheet.Cells[curRow, 3].Value = "Đạt";
        worksheet.Cells[curRow, 4].Value = "Không đạt";
        worksheet.Cells[curRow, 5].Value = "Ghi chú hiện trường";

        for (int c = 1; c <= 5; c++)
        {
            var headerStyle = worksheet.Cells[curRow, c].Style;
            headerStyle.Font.Bold = true;
            headerStyle.Fill.PatternType = ExcelFillStyle.Solid;
            headerStyle.Fill.BackgroundColor.SetColor(Color.FromArgb(31, 78, 120));
            headerStyle.Font.Color.SetColor(Color.White);
            headerStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headerStyle.VerticalAlignment = ExcelVerticalAlignment.Center;
            headerStyle.Border.BorderAround(ExcelBorderStyle.Thin);
        }
        worksheet.Row(curRow).Height = 24;

        // Checklist Data Rows
        int stt = 1;
        foreach (var item in phieu.Checklists)
        {
            curRow++;

            // STT
            worksheet.Cells[curRow, 1].Value = stt++;
            worksheet.Cells[curRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[curRow, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            // Nội dung
            worksheet.Cells[curRow, 2].Value = item.NoiDungChecklist;
            worksheet.Cells[curRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[curRow, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            worksheet.Cells[curRow, 2].Style.WrapText = true;

            // Đạt
            worksheet.Cells[curRow, 3].Value = item.DatYeuCau == true ? "[ X ]" : "[   ]";
            worksheet.Cells[curRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[curRow, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            // Không Đạt
            worksheet.Cells[curRow, 4].Value = item.DatYeuCau == false ? "[ X ]" : "[   ]";
            worksheet.Cells[curRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[curRow, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            // Ghi chú
            worksheet.Cells[curRow, 5].Value = item.GhiChuThucTe ?? "";
            worksheet.Cells[curRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[curRow, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            worksheet.Cells[curRow, 5].Style.WrapText = true;

            // Alternating rows coloring
            if (stt % 2 == 0)
            {
                for (int c = 1; c <= 5; c++)
                {
                    worksheet.Cells[curRow, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[curRow, c].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 247, 250));
                }
            }
        }

        // Section Title: Materials & Cost
        curRow += 2;
        var secTitleMaterials = worksheet.Cells[curRow, 1, curRow, 5];
        secTitleMaterials.Merge = true;
        secTitleMaterials.Value = "II. VẬT TƯ TIÊU HAO / THAY THẾ THỰC TẾ";
        secTitleMaterials.Style.Font.Bold = true;
        secTitleMaterials.Style.Font.Size = 11;
        secTitleMaterials.Style.Font.Color.SetColor(Color.FromArgb(31, 78, 120));

        // Materials Table Header
        curRow++;
        worksheet.Cells[curRow, 1].Value = "STT";
        worksheet.Cells[curRow, 2].Value = "Tên vật tư / Linh kiện";
        worksheet.Cells[curRow, 3].Value = "Số lượng";
        worksheet.Cells[curRow, 4].Value = "Đơn giá";
        worksheet.Cells[curRow, 5].Value = "Thành tiền";

        for (int c = 1; c <= 5; c++)
        {
            var headerStyle = worksheet.Cells[curRow, c].Style;
            headerStyle.Font.Bold = true;
            headerStyle.Fill.PatternType = ExcelFillStyle.Solid;
            headerStyle.Fill.BackgroundColor.SetColor(Color.FromArgb(56, 86, 35)); // Greenish header
            headerStyle.Font.Color.SetColor(Color.White);
            headerStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headerStyle.VerticalAlignment = ExcelVerticalAlignment.Center;
            headerStyle.Border.BorderAround(ExcelBorderStyle.Thin);
        }
        worksheet.Row(curRow).Height = 24;

        // Material Data Rows
        int mStt = 1;
        if (phieu.VatTus.Any())
        {
            foreach (var mat in phieu.VatTus)
            {
                curRow++;

                worksheet.Cells[curRow, 1].Value = mStt++;
                worksheet.Cells[curRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[curRow, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[curRow, 2].Value = mat.TenVatTu;
                worksheet.Cells[curRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[curRow, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[curRow, 2].Style.WrapText = true;

                worksheet.Cells[curRow, 3].Value = mat.SoLuong;
                worksheet.Cells[curRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[curRow, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[curRow, 4].Value = mat.DonGia;
                worksheet.Cells[curRow, 4].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[curRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[curRow, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[curRow, 5].Value = mat.ThanhTien;
                worksheet.Cells[curRow, 5].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[curRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[curRow, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }
        }

        // Add 3 blank write-in rows for manual entry at site
        for (int i = 0; i < 3; i++)
        {
            curRow++;
            worksheet.Row(curRow).Height = 22;

            worksheet.Cells[curRow, 1].Value = mStt++;
            worksheet.Cells[curRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[curRow, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            worksheet.Cells[curRow, 2].Value = "....................................................................................";
            worksheet.Cells[curRow, 2].Style.Font.Color.SetColor(Color.Gray);
            worksheet.Cells[curRow, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            worksheet.Cells[curRow, 3].Value = "............";
            worksheet.Cells[curRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[curRow, 3].Style.Font.Color.SetColor(Color.Gray);
            worksheet.Cells[curRow, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            worksheet.Cells[curRow, 4].Value = "............";
            worksheet.Cells[curRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[curRow, 4].Style.Font.Color.SetColor(Color.Gray);
            worksheet.Cells[curRow, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            worksheet.Cells[curRow, 5].Value = "............";
            worksheet.Cells[curRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[curRow, 5].Style.Font.Color.SetColor(Color.Gray);
            worksheet.Cells[curRow, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        }

        // Display Total cost if any
        if (phieu.ChiPhiThucTe.HasValue && phieu.ChiPhiThucTe.Value > 0)
        {
            curRow++;
            worksheet.Cells[curRow, 2].Value = "Tổng chi phí thực tế:";
            worksheet.Cells[curRow, 2].Style.Font.Bold = true;
            worksheet.Cells[curRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            worksheet.Cells[curRow, 5].Value = phieu.ChiPhiThucTe.Value;
            worksheet.Cells[curRow, 5].Style.Font.Bold = true;
            worksheet.Cells[curRow, 5].Style.Numberformat.Format = "#,##0";
            worksheet.Cells[curRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[curRow, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
        }

        // Section Title: Observations / Summary
        curRow += 2;
        var labelCell = worksheet.Cells[curRow, 1, curRow, 5];
        labelCell.Merge = true;
        labelCell.Value = "Ghi chú xử lý / Lý do hủy:";
        labelCell.Style.Font.Bold = true;
        
        curRow++;
        var notesCell = worksheet.Cells[curRow, 1, curRow + 2, 5];
        notesCell.Merge = true;
        notesCell.Value = (phieu.GhiChuXuLy ?? "") + (string.IsNullOrEmpty(phieu.LyDoHuy) ? "" : $" [Lý do hủy: {phieu.LyDoHuy}]");
        notesCell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
        notesCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
        notesCell.Style.WrapText = true;
        worksheet.Row(curRow).Height = 20;
        worksheet.Row(curRow + 1).Height = 20;
        worksheet.Row(curRow + 2).Height = 20;

        // Signatures Block
        curRow += 4;
        worksheet.Row(curRow).Height = 20;

        // Column B: Tech
        var signTech = worksheet.Cells[curRow, 2];
        signTech.Value = "KỸ THUẬT VIÊN";
        signTech.Style.Font.Bold = true;
        signTech.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        var subTech = worksheet.Cells[curRow + 1, 2];
        subTech.Value = "(Ký, ghi rõ họ tên)";
        subTech.Style.Font.Italic = true;
        subTech.Style.Font.Size = 9;
        subTech.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        // Column C & D: Partner
        var signPartner = worksheet.Cells[curRow, 3, curRow, 4];
        signPartner.Merge = true;
        signPartner.Value = "ĐỐI TÁC GIÁM SÁT";
        signPartner.Style.Font.Bold = true;
        signPartner.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        var subPartner = worksheet.Cells[curRow + 1, 3, curRow + 1, 4];
        subPartner.Merge = true;
        subPartner.Value = "(Ký, đóng dấu nếu có)";
        subPartner.Style.Font.Italic = true;
        subPartner.Style.Font.Size = 9;
        subPartner.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        // Column E: Management
        var signMgr = worksheet.Cells[curRow, 5];
        signMgr.Value = "BAN QUẢN LÝ NGHIỆM THU";
        signMgr.Style.Font.Bold = true;
        signMgr.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        var subMgr = worksheet.Cells[curRow + 1, 5];
        subMgr.Value = "(Ký, ghi rõ họ tên)";
        subMgr.Style.Font.Italic = true;
        subMgr.Style.Font.Size = 9;
        subMgr.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        // Pad space for physical signature
        curRow += 6;
        worksheet.Row(curRow).Height = 20;
        
        // Show names if available in DB
        if (phieu.NhanSuBaoTris.Any())
        {
            worksheet.Cells[curRow, 2].Value = phieu.NhanSuBaoTris.First().HoTen;
            worksheet.Cells[curRow, 2].Style.Font.Bold = true;
            worksheet.Cells[curRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
        if (!string.IsNullOrEmpty(phieu.TenNguoiKiemDuyet))
        {
            worksheet.Cells[curRow, 5].Value = phieu.TenNguoiKiemDuyet;
            worksheet.Cells[curRow, 5].Style.Font.Bold = true;
            worksheet.Cells[curRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        // Set column widths precisely for printable format
        worksheet.Column(1).Width = 8;   // STT
        worksheet.Column(2).Width = 48;  // Nội dung
        worksheet.Column(3).Width = 11;  // Đạt
        worksheet.Column(4).Width = 11;  // Không đạt
        worksheet.Column(5).Width = 35;  // Ghi chú

        return package.GetAsByteArray();
    }
}
