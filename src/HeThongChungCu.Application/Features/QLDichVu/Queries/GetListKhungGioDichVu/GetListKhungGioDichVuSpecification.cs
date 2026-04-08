using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetListKhungGioDichVu;

public class GetListKhungGioDichVuSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "TenKhungGio", "GioBatDau", "GioKetThuc", "NgayTrongTuan"
    };

    public GetListKhungGioDichVuSpecification(
        int? dichVuId,
        string? keyword,
        int? pageNumber = 1,
        int? pageSize = 10,
        string? sortBy = null,
        bool? isAsc = true)
        : base(sortBy, isAsc, pageNumber, pageSize)
    {
        AddFilter("IsDeleted", FilterOperator.Equal, false);

        if (dichVuId.HasValue)
        {
            AddFilter("DichVuId", FilterOperator.Equal, dichVuId.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("TenKhungGio", FilterOperator.Contains, keyword);
        }
    }
}
