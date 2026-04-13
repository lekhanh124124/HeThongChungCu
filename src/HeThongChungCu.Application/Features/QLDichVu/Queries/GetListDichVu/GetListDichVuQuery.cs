using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetListDichVu;

public record GetListDichVuQuery(
    int? LoaiDichVuId,
    int? DoiTacId,
    int? HopDongDoiTacId,
    bool? IsBatBuoc,
    int? TrangThaiDichVuId,
    string? Keyword,
    int? PageNumber,
    int? PageSize,
    string? SortCol,
    bool? IsAsc) : IQuery<PagedResult<DichVuResponse>>;
