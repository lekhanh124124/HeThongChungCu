using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

namespace HeThongChungCu.Application.Features.QLDoiTac.Queries.GetListHoaDonDoiTac;

public class GetListHoaDonDoiTacSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id",
        "Thang",
        "Nam",
        "SoTien",
        "NgayGhiNhan",
        "TrangThaiThanhToanId"
    };

    public GetListHoaDonDoiTacSpecification(
        int? doiTacId,
        int? hopDongDoiTacId,
        int? thang,
        int? nam,
        int? trangThaiThanhToanId,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize)
        : base(sortCol, isAsc, pageNumber, pageSize)
    {
        AddFilter("IsDeleted", FilterOperator.Equal, false);

        if (doiTacId.HasValue)
        {
            AddFilter("DoiTacId", FilterOperator.Equal, doiTacId.Value);
        }

        if (hopDongDoiTacId.HasValue)
        {
            AddFilter("HopDongDoiTacId", FilterOperator.Equal, hopDongDoiTacId.Value);
        }

        if (thang.HasValue)
        {
            AddFilter("Thang", FilterOperator.Equal, thang.Value);
        }

        if (nam.HasValue)
        {
            AddFilter("Nam", FilterOperator.Equal, nam.Value);
        }

        if (trangThaiThanhToanId.HasValue)
        {
            AddFilter("TrangThaiThanhToanId", FilterOperator.Equal, trangThaiThanhToanId.Value);
        }
    }
}
