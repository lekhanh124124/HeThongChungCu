using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayDSCuDanTrongChungCu;

public record LayDSCuDanTrongChungCuQuery(
    int? ToaNhaId,
    int? TangId,
    int? CanHoId,
    string? Keyword,
    string? MaToaNha,
    string? MaTang,
    string? MaCanHo,
    int? LoaiQuanHeCuTruId,
    bool? IsKetThuc,
    DateTime? NgayBatDauFrom,
    DateTime? NgayBatDauTo,
    DateTime? NgayKetThucFrom,
    DateTime? NgayKetThucTo,
    string? SortCol,
    bool? IsAsc,
    int? PageNumber,
    int? PageSize) : IQuery<PagedResult<CuDanResponse>>;
