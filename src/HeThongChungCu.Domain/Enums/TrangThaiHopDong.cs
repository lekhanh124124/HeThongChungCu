using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiHopDong : BaseEnum<TrangThaiHopDong, int>
{
    public static readonly TrangThaiHopDong ChuaKy = new(1, "Chưa ký");
    public static readonly TrangThaiHopDong HieuLuc = new(2, "Hiệu lực");
    public static readonly TrangThaiHopDong SapHetHan = new(3, "Sắp hết hạn");
    public static readonly TrangThaiHopDong HetHan = new(4, "Hết hạn");
    public static readonly TrangThaiHopDong TamNgung = new(5, "Tạm ngưng");

    private TrangThaiHopDong(int value, string name) : base(value, name)
    {
    }
}
