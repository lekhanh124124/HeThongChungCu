using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetListDangKyDichVu;

public record GetListDangKyDichVuQuery(
    int? LoaiDichVuId,
    int? DichVuId,
    int? TrangThaiDangKyId,
    DateTimeOffset? TuNgay,
    DateTimeOffset? DenNgay,
    string? Keyword,
    int? PageNumber,
    int? PageSize,
    string? SortCol,
    bool? IsAsc) : IQuery<PagedResult<DangKyDichVuResponse>>;
