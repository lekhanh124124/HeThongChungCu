using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetListDotThanhToan;

public class GetListDotThanhToanSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "TenDot", "Thang", "Nam", "TrangThaiDotThanhToanId", "NgayPhatHanh"
    };

    public GetListDotThanhToanSpecification(
        int? thang,
        int? nam,
        int? trangThaiId,
        string? keyword,
        int? pageNumber = 1,
        int? pageSize = 10,
        string? sortBy = null,
        bool? isAsc = false)
        : base(sortBy, isAsc, pageNumber, pageSize)
    {
        AddFilter("IsDeleted", FilterOperator.Equal, false);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("TenDot", FilterOperator.Contains, keyword);
            AddKeyword("GhiChu", FilterOperator.Contains, keyword);
        }

        if (thang.HasValue)
        {
            AddFilter("Thang", FilterOperator.Equal, thang.Value);
        }

        if (nam.HasValue)
        {
            AddFilter("Nam", FilterOperator.Equal, nam.Value);
        }

        if (trangThaiId.HasValue)
        {
            AddFilter("TrangThaiDotThanhToanId", FilterOperator.Equal, trangThaiId.Value);
        }
    }
}
