using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.NhanVien.Queries.GetNhanVienList;

public class GetNhanVienListSpecification : BaseSpecification
{
    public GetNhanVienListSpecification(
        string? keyword,
        int? loaiNhanVienId,
        int? trangThaiNhanVienId,
        int? pageIndex,
        int? pageSize)
        : base(null, null, pageIndex, pageSize)
    {
        if (!string.IsNullOrWhiteSpace(keyword))
            AddKeyword("Keyword", FilterOperator.Contains, keyword);

        if (loaiNhanVienId.HasValue)
            AddFilter("LoaiNhanVienId", FilterOperator.Equal, loaiNhanVienId.Value);

        if (trangThaiNhanVienId.HasValue)
            AddFilter("TrangThaiNhanVienId", FilterOperator.Equal, trangThaiNhanVienId.Value);

        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
