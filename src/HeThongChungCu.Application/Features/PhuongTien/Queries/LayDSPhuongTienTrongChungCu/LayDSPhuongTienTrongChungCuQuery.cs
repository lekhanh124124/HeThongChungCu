using HeThongChungCu.Application.Features.PhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.PhuongTien.Queries.LayDSPhuongTienTrongChungCu;

public record LayDSPhuongTienTrongChungCuQuery(
    int? ToaNhaId,
    int? TangId,
    int? CanHoId,
    string? Keyword,
    string? MaToaNha,
    string? MaTang,
    string? MaCanHo,
    int? LoaiPhuongTienId,
    string? MauXe,
    int? TrangThaiPhuongTienId,
    string? SortCol,
    bool? IsAsc,
    int? PageNumber,
    int? PageSize) : IQuery<PagedResult<PhuongTienResponse>>;


