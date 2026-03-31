using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiThongBao : BaseEnum<LoaiThongBao, int>
{
    public static readonly LoaiThongBao YeuCauCuTru = new(1, "Yêu cầu cư trú");
    public static readonly LoaiThongBao YeuCauPhuongTien = new(2, "Yêu cầu phương tiện");
    public static readonly LoaiThongBao ThanhToan = new(3, "Thanh toán");
    public static readonly LoaiThongBao HeThong = new(4, "Hệ thống");
    public static readonly LoaiThongBao Khac = new(5, "Khác");

    private LoaiThongBao(int value, string name) : base(value, name)
    {
    }
}
