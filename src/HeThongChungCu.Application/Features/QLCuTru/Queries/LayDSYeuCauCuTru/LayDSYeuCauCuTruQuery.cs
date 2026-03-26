using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSYeuCauCuTru;

public record LayDSYeuCauCuTruQuery(
    int? CanHoId,
    int? LoaiYeuCauId,
    int? TrangThaiId,
    string? SortCol,
    bool? IsAsc,
    int? PageNumber,
    int? PageSize) : IQuery<PagedResult<YeuCauCuTruResponse>>;
