using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayLichSuCuTru;

public record LayLichSuCuTruQuery(
    int UserId,
    int? LoaiQuanHeCuTruId,
    DateOnly? NgayBatDauFrom,
    DateOnly? NgayBatDauTo,
    DateOnly? NgayKetThucFrom,
    DateOnly? NgayKetThucTo,
    string? SortCol,
    bool? IsAsc,
    int? PageNumber,
    int? PageSize) : IQuery<PagedResult<LichSuCuTruResponse>>;
