using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiDinhGia : BaseEnum<LoaiDinhGia, int>
{
    public static readonly LoaiDinhGia CoDinh = new(1, "Cố định");
    public static readonly LoaiDinhGia LuyTien = new(2, "Lũy tiến");
    public static readonly LoaiDinhGia TheoBlock = new(3, "Theo Block");
    public static readonly LoaiDinhGia TheoGio = new(4, "Theo giờ");
    public static readonly LoaiDinhGia TheoMua = new(5, "Theo mùa");

    private LoaiDinhGia(int value, string name) : base(value, name)
    {
    }
}
