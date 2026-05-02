using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetListHoaDon;

public class GetListHoaDonSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "MaHoaDon", "NgayLap", "NgayHanThanhToan", "TongTien", "TrangThaiHoaDonId", "Thang", "Nam"
    };

    public GetListHoaDonSpecification(
        int? canHoId,
        int? dotThanhToanId,
        int? trangThaiHoaDonId,
        int? thang,
        int? nam,
        string? keyword,
        int? pageNumber = 1,
        int? pageSize = 10,
        string? sortBy = null,
        bool? isAsc = false)
        : base(sortBy, isAsc, pageNumber, pageSize)
    {
        if (canHoId.HasValue)
        {
            AddFilter("CanHoId", FilterOperator.Equal, canHoId.Value);
        }

        if (dotThanhToanId.HasValue)
        {
            AddFilter("DotThanhToanId", FilterOperator.Equal, dotThanhToanId.Value);
        }

        if (trangThaiHoaDonId.HasValue)
        {
            AddFilter("TrangThaiHoaDonId", FilterOperator.Equal, trangThaiHoaDonId.Value);
        }

        if (thang.HasValue)
        {
            AddFilter("Thang", FilterOperator.Equal, thang.Value);
        }

        if (nam.HasValue)
        {
            AddFilter("Nam", FilterOperator.Equal, nam.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("MaHoaDon", FilterOperator.Contains, keyword);
        }
    }
}
