using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiPhanAnh : BaseEnum<LoaiPhanAnh, int>
{
    public int HanXuLyGio { get; }

    public static readonly LoaiPhanAnh VeSinhMoitruong = new(1, "Vệ sinh & Môi trường", 24);
    public static readonly LoaiPhanAnh AnNinhBaoVe = new(2, "An ninh & Bảo vệ", 4);
    public static readonly LoaiPhanAnh HaTangKyThuat = new(3, "Hạ tầng & Kỹ thuật", 12);
    public static readonly LoaiPhanAnh ThaiDoPhucVu = new(4, "Thái độ phục vụ", 48);
    public static readonly LoaiPhanAnh TaiChinhPhiDichVu = new(5, "Tài chính & Phí dịch vụ", 24);
    public static readonly LoaiPhanAnh Khac = new(6, "Khác", 48);

    private LoaiPhanAnh(int value, string name, int hanXuLyGio) : base(value, name)
    {
        HanXuLyGio = hanXuLyGio;
    }
}
