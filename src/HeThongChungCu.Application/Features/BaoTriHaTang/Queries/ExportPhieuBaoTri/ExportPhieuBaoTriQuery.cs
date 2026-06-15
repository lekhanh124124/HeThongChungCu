using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.ExportPhieuBaoTri;

public record ExportPhieuBaoTriQuery(int Id) : IQuery<ExportExcelResponse>;
