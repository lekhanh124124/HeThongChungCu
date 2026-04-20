using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetListDangKyDichVu;

public class GetListDangKyDichVuSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "NgayBatDau", "TrangThaiDangKyId", "DichVuId", "SoLuong"
    };

    public GetListDangKyDichVuSpecification(
        int nguoiDungId,
        int? loaiDichVuId,
        int? dichVuId,
        int? trangThaiDangKyId,
        DateTimeOffset? tuNgay,
        DateTimeOffset? denNgay,
        string? keyword,
        int? pageNumber = 1,
        int? pageSize = 10,
        string? sortBy = null,
        bool? isAsc = false)
        : base(sortBy, isAsc, pageNumber, pageSize)
    {
        AddFilter("NguoiDungId", FilterOperator.Equal, nguoiDungId);
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

        if (dichVuId.HasValue)
        {
            AddFilter("DichVuId", FilterOperator.Equal, dichVuId.Value);
        }

        if (trangThaiDangKyId.HasValue)
        {
            AddFilter("TrangThaiDangKyId", FilterOperator.Equal, trangThaiDangKyId.Value);
        }

        if (tuNgay.HasValue)
        {
            AddFilter("TuNgay", FilterOperator.GreaterThanOrEqual, tuNgay.Value);
        }

        if (denNgay.HasValue)
        {
            AddFilter("DenNgay", FilterOperator.LessThanOrEqual, denNgay.Value);
        }
    }
}
