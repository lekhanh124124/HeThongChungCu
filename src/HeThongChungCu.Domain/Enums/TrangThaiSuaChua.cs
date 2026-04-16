using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiSuaChua : BaseEnum<TrangThaiSuaChua, int>
{
    public static readonly TrangThaiSuaChua MoiTao = new(1, "Mới tạo");
    public static readonly TrangThaiSuaChua DaTiepNhan = new(2, "Đã tiếp nhận");
    public static readonly TrangThaiSuaChua CanBoSungThongTin = new(3, "Cần bổ sung thông tin");
    public static readonly TrangThaiSuaChua DaDieuPhoi = new(4, "Đã điều phối");
    public static readonly TrangThaiSuaChua ChoKiemTra = new(10, "Chờ kiểm tra");
    public static readonly TrangThaiSuaChua ChoBaoGia = new(11, "Chờ báo giá");
    public static readonly TrangThaiSuaChua ChoCuDanDuyetBaoGia = new(12, "Chờ cư dân duyệt báo giá");
    public static readonly TrangThaiSuaChua DaDuyetBaoGia = new(13, "Đã duyệt báo giá");
    public static readonly TrangThaiSuaChua DaHenLich = new(5, "Đã hẹn lịch");
    public static readonly TrangThaiSuaChua DangXuLy = new(6, "Đang xử lý");
    public static readonly TrangThaiSuaChua DaXuLy = new(7, "Đã xử lý");
    public static readonly TrangThaiSuaChua DaDong = new(8, "Đã đóng");
    public static readonly TrangThaiSuaChua DaHuy = new(9, "Đã hủy");

    private TrangThaiSuaChua(int value, string name) : base(value, name)
    {
    }
}
