using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiPhanAnh : BaseEnum<TrangThaiPhanAnh, int>
{
    public static readonly TrangThaiPhanAnh ChoTiepNhan = new(1, "Chờ tiếp nhận");
    public static readonly TrangThaiPhanAnh DangXuLy = new(2, "Đang xử lý");
    public static readonly TrangThaiPhanAnh CSKHPhanHoi = new(3, "BQL đã phản hồi");
    public static readonly TrangThaiPhanAnh CuDanPhanHoi = new(4, "Cư dân đã phản hồi");
    public static readonly TrangThaiPhanAnh ChoDanhGia = new(5, "Chờ đánh giá");
    public static readonly TrangThaiPhanAnh DaDong = new(6, "Đã hoàn thành");
    public static readonly TrangThaiPhanAnh DaHuy = new(7, "Đã hủy");
    public static readonly TrangThaiPhanAnh Nhap = new(8, "Nháp");
    public static readonly TrangThaiPhanAnh DaThuHoi = new(9, "Đã thu hồi");

    private TrangThaiPhanAnh(int value, string name) : base(value, name) { }
}
