using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetListBangGia;

public class GetListBangGiaSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "TenBangGia", "NgayApDung", "NgayKetThuc", "LoaiDinhGiaId", "IsActive"
    };

    public GetListBangGiaSpecification(
        int? dichVuId,
        string? keyword,
        bool? isActive,
        int? pageNumber = 1,
        int? pageSize = 10,
        string? sortBy = null,
        bool? isAsc = false)
        : base(sortBy, isAsc, pageNumber, pageSize)
    {

        if (dichVuId.HasValue)
        {
            AddFilter("DichVuId", FilterOperator.Equal, dichVuId.Value);
        }

        if (isActive.HasValue)
        {
            AddFilter("IsActive", FilterOperator.Equal, isActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("TenBangGia", FilterOperator.Contains, keyword);
        }
    }
}
