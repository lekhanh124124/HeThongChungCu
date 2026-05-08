using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiKhaoSat : BaseEnum<LoaiKhaoSat, int>
{
    public static readonly LoaiKhaoSat LayYKienCuDan = new(1, "Khảo sát ý kiến thông thường");
    public static readonly LoaiKhaoSat BieuQuyetNghiQuyet = new(2, "Biểu quyết Nghị quyết tòa nhà");
    public static readonly LoaiKhaoSat BauCuBanQuanTri = new(3, "Bầu cử Ban Quản trị chung cư");

    private LoaiKhaoSat(int value, string name) : base(value, name) { }
}
