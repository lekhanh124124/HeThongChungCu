using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.NhanVien.DTOs;

namespace HeThongChungCu.Application.Features.NhanVien.Queries.GetNhanVienList;

public record GetNhanVienListQuery(
    string? Keyword,
    int? LoaiNhanVienId,
    int? TrangThaiNhanVienId,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PagedResult<NhanVienResponse>>;
