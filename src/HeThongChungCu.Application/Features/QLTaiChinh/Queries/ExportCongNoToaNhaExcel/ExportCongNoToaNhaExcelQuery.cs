using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.ExportCongNoToaNhaExcel;

public record ExportCongNoToaNhaExcelQuery : IQuery<ExportFileResponse>
{
    public int? Thang { get; init; }
    public int? Nam { get; init; }
}
