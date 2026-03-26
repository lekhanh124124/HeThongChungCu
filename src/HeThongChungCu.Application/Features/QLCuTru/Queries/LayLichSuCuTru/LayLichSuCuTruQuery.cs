using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.LayLichSuCuTru;

public record LayLichSuCuTruQuery(
    int UserId,
    int? LoaiQuanHeCuTruId,
    DateTime? NgayBatDauFrom,
    DateTime? NgayBatDauTo,
    DateTime? NgayKetThucFrom,
    DateTime? NgayKetThucTo,
    string? SortCol,
    bool? IsAsc,
    int? PageNumber,
    int? PageSize) : IQuery<PagedResult<LichSuCuTruResponse>>;
