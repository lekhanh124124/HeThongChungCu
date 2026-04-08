using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

namespace HeThongChungCu.Application.Features.QLDoiTac.Queries.GetListDoiTac;

public class GetListDoiTacSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id",
        "TenDoiTac",
        "TenCongTy",
        "NgayKyHopDong",
        "NgayHetHan"
    };

    public GetListDoiTacSpecification(
        string? keyword,
        int? trangThaiHopDongId,
        int? loaiDichVuId,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize) 
        : base(sortCol, isAsc, pageNumber, pageSize)
    {
        AddFilter("IsDeleted", FilterOperator.Equal, false);

        if (trangThaiHopDongId.HasValue)
        {
            AddFilter("TrangThaiHopDongId", FilterOperator.Equal, trangThaiHopDongId.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("TenDoiTac", FilterOperator.Contains, keyword);
            AddKeyword("TenCongTy", FilterOperator.Contains, keyword);
            AddKeyword("Email", FilterOperator.Contains, keyword);
            AddKeyword("SoDienThoai", FilterOperator.Contains, keyword);
        }

        if (loaiDichVuId.HasValue)
        {
            // Custom handle in repository using this property if needed,
            // or we add a proxy filter that repository maps.
            AddFilter("LoaiDichVuId", FilterOperator.Equal, loaiDichVuId.Value);
        }
    }
}
