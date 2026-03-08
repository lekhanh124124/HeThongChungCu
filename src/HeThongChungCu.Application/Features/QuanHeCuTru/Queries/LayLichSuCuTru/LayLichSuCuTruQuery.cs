using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayLichSuCuTru;

public record LayLichSuCuTruQuery(
    int? CanHoId = null,
    int? UserId = null,
    string? SortCol = null,
    bool? IsAsc = false,
    int? PageNumber = 1,
    int? PageSize = 20) : IQuery<PagedResult<LichSuCuTruResponse>>;
