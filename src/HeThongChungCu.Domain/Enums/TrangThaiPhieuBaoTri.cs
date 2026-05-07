using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiPhieuBaoTri : BaseEnum<TrangThaiPhieuBaoTri, int>
{
    public static readonly TrangThaiPhieuBaoTri ChoGiaoViec = new(1, "Chờ giao việc");
    public static readonly TrangThaiPhieuBaoTri DaGiaoViec = new(2, "Đã giao việc");
    public static readonly TrangThaiPhieuBaoTri DangThucHien = new(3, "Đang thực hiện");
    public static readonly TrangThaiPhieuBaoTri ChoNghiemThu = new(4, "Chờ nghiệm thu");
    public static readonly TrangThaiPhieuBaoTri DaHoanThanh = new(5, "Đã hoàn thành");
    public static readonly TrangThaiPhieuBaoTri DaHuy = new(6, "Đã hủy");

    private TrangThaiPhieuBaoTri(int value, string name) : base(value, name)
    {
    }
}
