using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetListChiSo;

public record GetListChiSoQuery(
    int? Thang = null,
    int? Nam = null,
    int? DichVuId = null,
    int? TrangThaiChiSoId = null,
    int? ToaNhaId = null,
    int? TangId = null,
    int? CanHoId = null,
    int? PageNumber = 1,
    int? PageSize = 10,
    string? SortCol = null,
    bool? IsAsc = false) : IQuery<PagedResult<ChiSoResponse>>;
