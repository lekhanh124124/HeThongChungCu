using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiPhuongTien : BaseEnum<LoaiPhuongTien, int>
{
    public string DefaultServiceCode { get; private set; }

    public static readonly LoaiPhuongTien XeMay = new(1, "Xe máy", "PK_MOTOR");
    public static readonly LoaiPhuongTien Oto = new(2, "Ô tô", "PK_CAR");
    public static readonly LoaiPhuongTien XeDap = new(3, "Xe đạp", "PK_BIKE");
    public static readonly LoaiPhuongTien XeDien = new(4, "Xe điện", "PK_EV");

    private LoaiPhuongTien(int value, string name, string defaultServiceCode) : base(value, name)
    {
        DefaultServiceCode = defaultServiceCode;
    }
}
