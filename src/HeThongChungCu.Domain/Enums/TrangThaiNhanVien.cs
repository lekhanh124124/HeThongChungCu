using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiNhanVien : BaseEnum<TrangThaiNhanVien, int>
{
    public static readonly TrangThaiNhanVien DangLamViec = new(1, "Đang làm việc");
    public static readonly TrangThaiNhanVien TamNghi = new(2, "Tạm nghỉ");
    public static readonly TrangThaiNhanVien DaNghiViec = new(3, "Đã nghỉ việc");

    private TrangThaiNhanVien(int value, string name) : base(value, name)
    {
    }
}
