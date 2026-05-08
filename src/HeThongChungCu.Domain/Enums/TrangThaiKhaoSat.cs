using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiKhaoSat : BaseEnum<TrangThaiKhaoSat, int>
{
    public static readonly TrangThaiKhaoSat MoiTao = new(1, "Mới tạo (Draft)");
    public static readonly TrangThaiKhaoSat DangDienRa = new(2, "Đang diễn ra (Published)");
    public static readonly TrangThaiKhaoSat TamDung = new(3, "Tạm dừng");
    public static readonly TrangThaiKhaoSat DaKetThuc = new(4, "Đã kết thúc");

    private TrangThaiKhaoSat(int value, string name) : base(value, name) { }
}
