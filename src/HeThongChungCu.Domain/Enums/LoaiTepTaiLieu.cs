using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiTepTaiLieu : BaseEnum<LoaiTepTaiLieu, int>
{
    public static readonly LoaiTepTaiLieu MacDinh = new(1, "Mặc định");
    public static readonly LoaiTepTaiLieu NguoiDung = new(2, "Người dùng");
    public static readonly LoaiTepTaiLieu YeuCauCuTru = new(3, "Yêu cầu cư trú");
    public static readonly LoaiTepTaiLieu YeuCauPhuongTien = new(4, "Yêu cầu phương tiện");
    public static readonly LoaiTepTaiLieu YeuCauSuaChua = new(5, "Yêu cầu sửa chữa");
    public static readonly LoaiTepTaiLieu YeuCauThiCong = new(6, "Yêu cầu thi công");
    public static readonly LoaiTepTaiLieu PhuongTien = new(7, "Phương tiện");
    public static readonly LoaiTepTaiLieu HopDongDoiTac = new(8, "Hợp đồng đối tác");
    public static readonly LoaiTepTaiLieu YeuCauPhanAnh = new(9, "Yêu cầu phản ánh");
    public static readonly LoaiTepTaiLieu SaoLuuDb = new(10, "Sao lưu dữ liệu nghiệp vụ");

    private LoaiTepTaiLieu(int value, string name) : base(value, name)
    {
    }
}
