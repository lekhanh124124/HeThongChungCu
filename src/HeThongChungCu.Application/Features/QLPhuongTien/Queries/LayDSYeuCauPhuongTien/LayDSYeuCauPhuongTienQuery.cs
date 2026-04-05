using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSYeuCauPhuongTien;

public record LayDSYeuCauPhuongTienQuery(
    int? ToaNhaId,
    int? TangId,
    int? CanHoId,
    int? LoaiYeuCauId,
    int? TrangThaiId,
    string? Keyword,
    string? SortCol,
    bool? IsAsc,
    int? PageNumber,
    int? PageSize) : IQuery<PagedResult<DSYeuCauPhuongTienResponse>>;
