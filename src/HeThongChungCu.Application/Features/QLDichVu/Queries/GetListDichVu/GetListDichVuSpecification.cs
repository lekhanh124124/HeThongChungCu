using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using System.Text.Json;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetListDichVu;

public class GetListDichVuSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "MaDichVu", "TenDichVu", "DonViTinh", "MoTa", "IsBatBuoc", "TrangThaiId", "LoaiDichVuId"
    };

    public GetListDichVuSpecification(
        int? loaiDichVuId,
        int? doiTacId,
        int? hopDongDoiTacId,
        bool? isBatBuoc,
        int? trangThaiDichVuId,
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
            AddKeyword("TenDichVu", FilterOperator.Contains, keyword);
            AddKeyword("MaDichVu", FilterOperator.Contains, keyword);
        }

        if (loaiDichVuId.HasValue)
        {
            AddFilter("LoaiDichVuId", FilterOperator.Equal, loaiDichVuId.Value);
        }

        if (doiTacId.HasValue)
        {
            AddFilter("DoiTacId", FilterOperator.Equal, doiTacId.Value);
        }

        if (hopDongDoiTacId.HasValue)
        {
            AddFilter("HopDongDoiTacId", FilterOperator.Equal, hopDongDoiTacId.Value);
        }

        if (isBatBuoc.HasValue)
        {
            AddFilter("IsBatBuoc", FilterOperator.Equal, isBatBuoc.Value);
        }

        if (trangThaiDichVuId.HasValue)
        {
            AddFilter("TrangThaiDichVuId", FilterOperator.Equal, trangThaiDichVuId.Value);
        }
    }
}
