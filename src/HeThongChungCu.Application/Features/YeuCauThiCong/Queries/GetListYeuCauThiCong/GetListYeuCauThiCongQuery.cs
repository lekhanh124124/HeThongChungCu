using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;
using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetListYeuCauThiCong;

public record GetListYeuCauThiCongQuery(
    int? CanHoId,
    int? TrangThaiId,
    int? TrangThaiThiCongId,
    string? Keyword,
    DateTimeOffset? NgayTaoTu,
    DateTimeOffset? NgayTaoDen,
    DateTimeOffset? BatDauTu,
    DateTimeOffset? BatDauDen,
    DateTimeOffset? KetThucTu,
    DateTimeOffset? KetThucDen,
    string? SortCol,
    bool? IsAsc,
    int? PageNumber,
    int? PageSize) : IQuery<PagedResult<YeuCauThiCongResponse>>;
