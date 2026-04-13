using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class NgayTrongTuan : BaseEnum<NgayTrongTuan, int>
{
    public static readonly NgayTrongTuan ChuNhat = new(0, "Chủ Nhật");
    public static readonly NgayTrongTuan ThuHai = new(1, "Thứ Hai");
    public static readonly NgayTrongTuan ThuBa = new(2, "Thứ Ba");
    public static readonly NgayTrongTuan ThuTu = new(3, "Thứ Tư");
    public static readonly NgayTrongTuan ThuNam = new(4, "Thứ Năm");
    public static readonly NgayTrongTuan ThuSau = new(5, "Thứ Sáu");
    public static readonly NgayTrongTuan ThuBay = new(6, "Thứ Bảy");

    private NgayTrongTuan(int value, string name) : base(value, name)
    {
    }

    public static implicit operator int(NgayTrongTuan day) => day.Value;
}
