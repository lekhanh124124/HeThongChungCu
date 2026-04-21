using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiThiCong : BaseEnum<TrangThaiThiCong, int>
{
    public static readonly TrangThaiThiCong ChuaThiCong = new(1, "Chưa thi công");
    public static readonly TrangThaiThiCong ChoBoSungHoSo = new(2, "Chờ bổ sung hồ sơ");
    public static readonly TrangThaiThiCong ChoDuyetChinhThuc = new(3, "Chờ duyệt chính thức");
    public static readonly TrangThaiThiCong ChoThuCoc = new(4, "Chờ thu tiền cọc");
    public static readonly TrangThaiThiCong DaCapPhep = new(5, "Đã cấp phép");
    public static readonly TrangThaiThiCong DangThiCong = new(6, "Đang thi công");
    public static readonly TrangThaiThiCong DaHoanTat = new(7, "Đã hoàn tất");

    private TrangThaiThiCong(int value, string name) : base(value, name)
    {
    }
}
