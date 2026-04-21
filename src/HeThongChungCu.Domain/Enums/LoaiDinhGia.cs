using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiDinhGia : BaseEnum<LoaiDinhGia, int>
{
    public string Code { get; init; }

    public static readonly LoaiDinhGia CoDinh = new(1, nameof(CoDinh), "Cố định");
    public static readonly LoaiDinhGia LuyTien = new(2, nameof(LuyTien), "Lũy tiến");
    // public static readonly LoaiDinhGia TheoSoLuong = new(3, nameof(TheoSoLuong), "Theo số lượng");
    // public static readonly LoaiDinhGia TheoPhanTram = new(4, nameof(TheoPhanTram), "Theo phần trăm");
    // public static readonly LoaiDinhGia TheoChiSo = new(5, nameof(TheoChiSo), "Theo chỉ số");
    public static readonly LoaiDinhGia TheoDienTich = new(6, nameof(TheoDienTich), "Theo diện tích");
    public static readonly LoaiDinhGia TheoKhungGio = new(7, nameof(TheoKhungGio), "Theo khung giờ");

    private LoaiDinhGia(int value, string code, string name) : base(value, name)
    {
        Code = code;
    }
}
