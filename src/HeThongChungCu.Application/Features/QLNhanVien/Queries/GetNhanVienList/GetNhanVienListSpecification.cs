using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLNhanVien.Queries.GetNhanVienList;

public class GetNhanVienListSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id",
        "HoTen",
        "Email",
        "SoDienThoai",
        "LoaiNhanVienId",
        "TrangThaiNhanVienId",
        "NgayVaoLam",
        "NgayNghiLam"
    };

    public GetNhanVienListSpecification(
        string? keyword,
        int? loaiNhanVienId,
        int? trangThaiNhanVienId,
        string? sortCol,
        bool? isAsc,
        int? pageIndex,
        int? pageSize)
        : base(sortCol, isAsc, pageIndex, pageSize)
    {
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("HoTen", FilterOperator.Contains, keyword);
            AddKeyword("Email", FilterOperator.Contains, keyword);
            AddKeyword("SoDienThoai", FilterOperator.Contains, keyword);
        }

        if (loaiNhanVienId.HasValue)
            AddFilter("LoaiNhanVienId", FilterOperator.Equal, loaiNhanVienId.Value);

        if (trangThaiNhanVienId.HasValue)
            AddFilter("TrangThaiNhanVienId", FilterOperator.Equal, trangThaiNhanVienId.Value);
    }
}
