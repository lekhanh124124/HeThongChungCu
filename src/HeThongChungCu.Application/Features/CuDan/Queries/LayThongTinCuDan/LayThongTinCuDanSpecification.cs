using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.CuDan.Queries.LayThongTinCuDan;

public class LayThongTinCuDanSpecification : BaseSpecification
{
    public LayThongTinCuDanSpecification(int quanHeCuTruId) 
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, quanHeCuTruId);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
        AddFilter("NguoiDungIsDeleted", FilterOperator.Equal, false);
        AddFilter("TaiKhoanIsActive", FilterOperator.Equal, true);
        AddFilter("TaiKhoanIsDeleted", FilterOperator.Equal, false);
        AddFilter("TepIsDeleted", FilterOperator.Equal, false);
        AddFilter("LoaiTepNguoiDung", FilterOperator.Equal, LoaiTepTaiLieu.MacDinh.Value);
        AddFilter("TaiLieuIsDeleted", FilterOperator.Equal, false);
        AddFilter("LoaiTaiLieuNguoiDung", FilterOperator.Equal, LoaiTaiLieu.NguoiDung.Value);
    }
}

