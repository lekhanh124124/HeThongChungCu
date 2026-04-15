using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLNhanVien.Queries.GetNhanVienById;

public class GetNhanVienByIdSpecification : BaseSpecification
{
    public GetNhanVienByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("IsDeleted", FilterOperator.Equal, false);

        // NguoiDung filters
        AddFilter("NguoiDungIsDeleted", FilterOperator.Equal, false);

        // Account filters
        AddFilter("TaiKhoanIsActive", FilterOperator.Equal, true);
        AddFilter("TaiKhoanIsDeleted", FilterOperator.Equal, false);

        // File filters
        AddFilter("TepIsDeleted", FilterOperator.Equal, false);
        AddFilter("LoaiTepTaiLieu", FilterOperator.Equal, "TepTaiLieu");

        // Document filters
        AddFilter("TaiLieuIsDeleted", FilterOperator.Equal, false);
        AddFilter("LoaiTaiLieu", FilterOperator.Equal, "TaiLieuNguoiDung");
        AddFilter("LoaiTepTaiLieuNguoiDung", FilterOperator.Equal, "TepTaiLieuNguoiDung");
    }
}
