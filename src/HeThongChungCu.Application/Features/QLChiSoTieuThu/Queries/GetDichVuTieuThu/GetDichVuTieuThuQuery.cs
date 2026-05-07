using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetDichVuTieuThu;

public record GetDichVuTieuThuQuery : IQuery<List<DichVuResponse>>;
