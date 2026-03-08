using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Enums;

public class GioiTinh : BaseEnum<GioiTinh, int>
{
    public static readonly GioiTinh Nam = new(1, "Nam");
    public static readonly GioiTinh Nu = new(2, "Nữ");
    public static readonly GioiTinh Khac = new(3, "Khác");

    private GioiTinh(int value, string name) : base(value, name)
    {
    }

    public static GioiTinh? FromValue(int value)
    {
        return FromValue(value, v => new BusinessException($"Không có giới tính nào với giá trị {v}."));
    }

    public static GioiTinh? FromName(string name)
    {
        return FromName(name, n => new BusinessException($"Không có giới tính nào với tên '{n}'."));
    }
}
