using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiThongBao : BaseEnum<LoaiThongBao, int>
{
    public static readonly LoaiThongBao YeuCauCuTru = new(1, "Yêu cầu cư trú");
    public static readonly LoaiThongBao YeuCauPhuongTien = new(2, "Yêu cầu phương tiện");
    public static readonly LoaiThongBao ThanhToan = new(3, "Thanh toán");
    public static readonly LoaiThongBao YeuCauThiCong = new(4, "Yêu cầu thi công");
    public static readonly LoaiThongBao HeThong = new(5, "Hệ thống");
    public static readonly LoaiThongBao Khac = new(6, "Khác");
    public static readonly LoaiThongBao YeuCauSuaChua = new(7, "Yêu cầu sửa chữa");

    private LoaiThongBao(int value, string name) : base(value, name)
    {
    }
}
