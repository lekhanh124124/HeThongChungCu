using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetListHoaDon;

public record GetListHoaDonQuery(
    int? CanHoId = null,
    int? DotThanhToanId = null,
    int? TrangThaiHoaDonId = null,
    int? NguoiDungId = null,
    int? Thang = null,
    int? Nam = null,
    string? Keyword = null,
    int? PageNumber = 1,
    int? PageSize = 10,
    string? SortCol = null,
    bool? IsAsc = false) : IQuery<PagedResult<HoaDonResponse>>;
