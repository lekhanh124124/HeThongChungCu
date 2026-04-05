using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSYeuCauCuTru;

public class LayDSYeuCauCuTruQuerySpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "CanHoId", "LoaiYeuCauId", "TrangThaiId", "CreatedAt", "ToaNhaId", "TangId"
    };

    public LayDSYeuCauCuTruQuerySpecification(
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
        }
            
        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
