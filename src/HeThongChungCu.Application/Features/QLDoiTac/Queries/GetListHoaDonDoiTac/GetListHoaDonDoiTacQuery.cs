using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;

namespace HeThongChungCu.Application.Features.QLDoiTac.Queries.GetListHoaDonDoiTac;

public record GetListHoaDonDoiTacQuery(
    int? DoiTacId,
    int? HopDongDoiTacId,
    int? Thang,
    int? Nam,
    int? TrangThaiThanhToanId,
    string? SortCol,
    bool? IsAsc,
    int? PageNumber,
    int? PageSize) : IQuery<PagedResult<HoaDonDoiTacResponse>>;
