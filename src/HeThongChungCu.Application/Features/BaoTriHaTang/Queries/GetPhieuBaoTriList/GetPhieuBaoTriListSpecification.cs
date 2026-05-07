using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetPhieuBaoTriList;

public class GetPhieuBaoTriListSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id",
        "MaPhieu",
        "ThietBiId",
        "HangMucBaoTriId",
        "NgayLapPhieu",
        "NgayDuKien",
        "NgayThucTe",
        "TrangThaiPhieuBaoTriId"
    };

    public GetPhieuBaoTriListSpecification(
        string? keyword,
        int? trangThaiPhieuBaoTriId,
        int? thietBiId,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize)
        : base(sortCol, isAsc, pageNumber, pageSize)
    {
        AddFilter("IsDeleted", FilterOperator.Equal, false);

        if (trangThaiPhieuBaoTriId.HasValue)
        {
            AddFilter("TrangThaiPhieuBaoTriId", FilterOperator.Equal, trangThaiPhieuBaoTriId.Value);
        }

        if (thietBiId.HasValue)
        {
            AddFilter("ThietBiId", FilterOperator.Equal, thietBiId.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("MaPhieu", FilterOperator.Contains, keyword);
            AddKeyword("GhiChuXuLy", FilterOperator.Contains, keyword);
        }
    }
}
