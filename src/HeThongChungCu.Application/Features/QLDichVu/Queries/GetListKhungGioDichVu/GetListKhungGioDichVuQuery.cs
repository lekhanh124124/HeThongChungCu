using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetListKhungGioDichVu;

public record GetListKhungGioDichVuQuery(
    int? DichVuId,
    string? Keyword,
    int? PageNumber,
    int? PageSize,
    string? SortCol,
    bool? IsAsc,
    bool? IsActive) : IQuery<PagedResult<KhungGioDichVuResponse>>;
