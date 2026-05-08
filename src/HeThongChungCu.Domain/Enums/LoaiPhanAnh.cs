using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiPhanAnh : BaseEnum<LoaiPhanAnh, int>
{
    public static readonly LoaiPhanAnh VeSinhMoitruong = new(1, "Vệ sinh & Môi trường");
    public static readonly LoaiPhanAnh AnNinhBaoVe = new(2, "An ninh & Bảo vệ");
    public static readonly LoaiPhanAnh HaTangKyThuat = new(3, "Hạ tầng & Kỹ thuật");
    public static readonly LoaiPhanAnh ThaiDoPhucVu = new(4, "Thái độ phục vụ");
    public static readonly LoaiPhanAnh TaiChinhPhiDichVu = new(5, "Tài chính & Phí dịch vụ");
    public static readonly LoaiPhanAnh Khac = new(6, "Khác");

    private LoaiPhanAnh(int value, string name) : base(value, name) { }
}
