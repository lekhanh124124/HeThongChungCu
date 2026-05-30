using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.CuDan.Queries.LayThanhVienCuTru;

public class LayThanhVienCuTruSpecification : BaseSpecification
{
    public LayThanhVienCuTruSpecification(int canHoId) : base(null, null, null, null)
    {
        AddFilter("CanHoId", FilterOperator.Equal, canHoId);
        AddFilter("TrangThaiCuTruId", FilterOperator.Equal, 1);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
        AddFilter("NguoiDungIsDeleted", FilterOperator.Equal, false);
        AddFilter("TaiKhoanIsActive", FilterOperator.Equal, true);
        AddFilter("TepIsDeleted", FilterOperator.Equal, false);
        AddFilter("LoaiTepNguoiDung", FilterOperator.Equal, LoaiTepTaiLieu.MacDinh.Value);
    }
}

