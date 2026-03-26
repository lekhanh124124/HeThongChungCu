using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSCuDanTrongChungCu;

public record LayDSCuDanTrongChungCuQuery(
    int? ToaNhaId,
    int? TangId,
    int? CanHoId,
    string? Keyword,
    string? MaToaNha,
    string? MaTang,
    string? MaCanHo,
    int? LoaiQuanHeCuTruId,
    int? TrangThaiCuTruId,
    DateTime? NgayBatDauFrom,
    DateTime? NgayBatDauTo,
    DateTime? NgayKetThucFrom,
    DateTime? NgayKetThucTo,
    string? SortCol,
    bool? IsAsc,
    int? PageNumber,
    int? PageSize) : IQuery<PagedResult<CuDanResponse>>;
