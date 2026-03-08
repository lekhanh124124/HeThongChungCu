using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiQuanHeCuTru : BaseEnum<LoaiQuanHeCuTru, int>
{
    public static readonly LoaiQuanHeCuTru ChuHo = new(1, "Chủ hộ");
    public static readonly LoaiQuanHeCuTru NguoiThue = new(2, "Người thuê");
    public static readonly LoaiQuanHeCuTru NguoiOCung = new(3, "Người ở cùng");
    public static readonly LoaiQuanHeCuTru Khac = new(4, "Khác");

    private LoaiQuanHeCuTru(int value, string name) : base(value, name)
    {
    }
}
