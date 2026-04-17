using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiYeuCauCuDan : BaseEnum<LoaiYeuCauCuDan, int>
{
    public static readonly LoaiYeuCauCuDan CuTru = new(1, "Cư trú");
    public static readonly LoaiYeuCauCuDan PhuongTien = new(2, "Phương tiện");
    public static readonly LoaiYeuCauCuDan SuaChua = new(3, "Sửa chữa");
    public static readonly LoaiYeuCauCuDan ThiCongNoiThat = new(4, "Thi công nội thất");

    private LoaiYeuCauCuDan(int value, string name) : base(value, name)
    {
    }
}
