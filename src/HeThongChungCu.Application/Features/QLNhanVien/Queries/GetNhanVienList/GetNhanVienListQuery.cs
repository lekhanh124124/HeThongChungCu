using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLNhanVien.DTOs;

namespace HeThongChungCu.Application.Features.QLNhanVien.Queries.GetNhanVienList;

public record GetNhanVienListQuery(
    string? Keyword,
    int? LoaiNhanVienId,
    int? TrangThaiNhanVienId,
    string? SortCol,
    bool? IsAsc,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PagedResult<NhanVienResponse>>;
