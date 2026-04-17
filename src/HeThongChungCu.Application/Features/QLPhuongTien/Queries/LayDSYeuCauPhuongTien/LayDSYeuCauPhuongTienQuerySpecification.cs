using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSYeuCauPhuongTien;

public class LayDSYeuCauPhuongTienQuerySpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "CanHoId", "LoaiYeuCauId", "TrangThaiId", "CreatedAt", "ToaNhaId", "TangId"
    };

    public LayDSYeuCauPhuongTienQuerySpecification(
        int? toaNhaId,
        int? tangId,
        int? canHoId,
        int? loaiYeuCauId,
        int? trangThaiId,
        string? keyword,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize) : base(sortCol, isAsc, pageNumber, pageSize)
    {
        AddFilter("IsDeleted", FilterOperator.Equal, false);
        AddFilter("LoaiYeuCauCuDan", FilterOperator.Equal, LoaiYeuCauCuDan.PhuongTien.Value);
        AddFilter("CanHoIsDeleted", FilterOperator.Equal, false);
        AddFilter("TangIsDeleted", FilterOperator.Equal, false);
        AddFilter("ToaNhaIsDeleted", FilterOperator.Equal, false);
        AddFilter("TaiKhoanIsActive", FilterOperator.Equal, true);
        AddFilter("TaiKhoanIsDeleted", FilterOperator.Equal, false);

        if (toaNhaId != null)
            AddFilter("ToaNhaId", FilterOperator.Equal, toaNhaId);

        if (tangId != null)
            AddFilter("TangId", FilterOperator.Equal, tangId);

        if (canHoId != null)
            AddFilter("CanHoId", FilterOperator.Equal, canHoId);

        if (loaiYeuCauId != null)
            AddFilter("LoaiYeuCauId", FilterOperator.Equal, loaiYeuCauId);

        if (trangThaiId != null)
            AddFilter("TrangThaiId", FilterOperator.Equal, trangThaiId);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("TenNguoiGui", FilterOperator.Contains, keyword);
            AddKeyword("TenNguoiXuLy", FilterOperator.Contains, keyword);
            AddKeyword("YeuCauBienSo", FilterOperator.Contains, keyword);
        }
    }
}
