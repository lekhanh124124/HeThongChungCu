using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiDinhGia : BaseEnum<LoaiDinhGia, int>
{
    public static readonly LoaiDinhGia CoDinh = new(1, "Cố định");
    public static readonly LoaiDinhGia LuyTien = new(2, "Lũy tiến");
    public static readonly LoaiDinhGia TheoSoLuong = new(3, "Theo số lượng");
    public static readonly LoaiDinhGia TheoPhanTram = new(4, "Theo phần trăm");
    public static readonly LoaiDinhGia TheoChiSo = new(5, "Theo chỉ số");
    public static readonly LoaiDinhGia TheoDienTich = new(6, "Theo diện tích");
    public static readonly LoaiDinhGia TheoKhungGio = new(7, "Theo khung giờ");

    private LoaiDinhGia(int value, string name) : base(value, name)
    {
    }
}
