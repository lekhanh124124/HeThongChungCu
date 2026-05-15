using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.ExportCongNoCanHoExcel;

public record ExportCongNoCanHoExcelQuery : IQuery<ExportFileResponse>
{
    public int? ToaNhaId { get; init; }
    public int? Thang { get; init; }
    public int? Nam { get; init; }
}
