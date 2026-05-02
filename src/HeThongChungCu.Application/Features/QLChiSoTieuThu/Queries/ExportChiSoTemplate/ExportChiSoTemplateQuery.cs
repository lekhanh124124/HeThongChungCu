using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.ExportChiSoTemplate;

public record ExportChiSoTemplateQuery(
    int? ToaNhaId, 
    int? TangId, 
    int DichVuId,
    int Thang,
    int Nam) : IQuery<ExportFileResponse>;
