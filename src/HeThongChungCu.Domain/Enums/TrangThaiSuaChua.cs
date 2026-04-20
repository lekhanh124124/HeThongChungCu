using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiSuaChua : BaseEnum<TrangThaiSuaChua, int>
{
    public static readonly TrangThaiSuaChua DaDieuPhoi = new(1, "Đã điều phối");
    public static readonly TrangThaiSuaChua ChoBaoGia = new(2, "Chờ báo giá");
    public static readonly TrangThaiSuaChua DaDuyetBaoGia = new(3, "Đã duyệt báo giá");
    public static readonly TrangThaiSuaChua DaHenLich = new(4, "Đã hẹn lịch");

    private TrangThaiSuaChua(int value, string name) : base(value, name)
    {
    }
}
