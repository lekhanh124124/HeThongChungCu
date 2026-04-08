using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetListKhungGioDichVu;

public record GetListKhungGioDichVuQuery(
    int? DichVuId = null,
    string? Keyword = null,
    int? PageNumber = 1,
    int? PageSize = 10,
    string? SortCol = "Id",
    bool? IsAsc = true) : IQuery<PagedResult<KhungGioDichVuResponse>>;
