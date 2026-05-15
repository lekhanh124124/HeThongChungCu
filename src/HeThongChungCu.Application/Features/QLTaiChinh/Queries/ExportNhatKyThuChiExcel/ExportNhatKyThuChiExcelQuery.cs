using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.Common.Messaging;
using System;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.ExportNhatKyThuChiExcel;

public record ExportNhatKyThuChiExcelQuery : IQuery<ExportFileResponse>
{
    public int? LoaiGiaoDichId { get; init; }
    public int? DichVuId { get; init; }
    public string? NhomThongKe { get; init; }
    public DateTimeOffset? TuNgay { get; init; }
    public DateTimeOffset? DenNgay { get; init; }
    public string? Keyword { get; init; }
}
