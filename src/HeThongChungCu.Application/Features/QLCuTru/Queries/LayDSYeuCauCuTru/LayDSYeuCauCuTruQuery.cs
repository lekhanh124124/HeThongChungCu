using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSYeuCauCuTru;

public record LayDSYeuCauCuTruQuery(
    int? ToaNhaId,
    int? TangId,
    int? CanHoId,
    int? LoaiYeuCauId,
    int? TrangThaiId,
    string? Keyword,
    string? SortCol,
    bool? IsAsc,
    int? PageNumber,
    int? PageSize) : IQuery<PagedResult<DSYeuCauCuTruResponse>>;
