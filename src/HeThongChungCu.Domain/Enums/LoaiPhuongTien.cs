using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiPhuongTien : BaseEnum<LoaiPhuongTien, int>
{
    public static readonly LoaiPhuongTien XeMay = new(1, "Xe máy");
    public static readonly LoaiPhuongTien Oto = new(2, "Ô tô");
    public static readonly LoaiPhuongTien XeDap = new(3, "Xe đạp");
    public static readonly LoaiPhuongTien XeDien = new(4, "Xe điện");

    private LoaiPhuongTien(int value, string name) : base(value, name)
    {
    }
}
