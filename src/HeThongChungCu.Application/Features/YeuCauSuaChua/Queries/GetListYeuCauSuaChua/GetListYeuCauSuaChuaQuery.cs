using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetListYeuCauSuaChua;

public record GetListYeuCauSuaChuaQuery : IQuery<PagedResult<YeuCauSuaChuaResponse>>
{
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
    public string? SortCol { get; init; }
    public bool? IsAsc { get; init; }

    public int? CanHoId { get; init; }
    public int? TrangThaiYeuCauId { get; init; }
    public int? TrangThaiSuaChuaId { get; init; }
    public int? LoaiSuCoId { get; init; }
    public DateTimeOffset? NgayTaoTu { get; init; }
    public DateTimeOffset? NgayTaoDen { get; init; }

    public string? MaCanHo { get; init; }
    public string? TenNguoiGui { get; init; }
}
