namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class CanHoErrors
{
    public static readonly Error NotFound = Error.NotFound("Căn hộ");

    public static readonly Error MaCanHoAlreadyExists = Error.AlreadyExists("căn hộ", "mã căn hộ", "");

    public static readonly Error TangKhongHopLe = new(
        "CanHo.TangKhongHopLe",
        "Tầng của căn hộ không hợp lệ so với quy mô của tòa nhà.");

    public static readonly Error CanHoInBasement = new(
        "CanHo.CanHoInBasement",
        "Không thể tạo căn hộ ở tầng hầm.");

    public static Error NotFoundById(int id) => Error.NotFound("Căn hộ", id);

    public static Error NotFoundByIds(IEnumerable<int> ids) => 
        Error.NotFound("Căn hộ", string.Join(", ", ids));

    public static Error InvalidType(IEnumerable<string> allowedValues) => 
        Error.InvalidType("Loại căn hộ", allowedValues);

    public static Error InvalidStatus(IEnumerable<string> allowedValues) => 
        Error.InvalidType("Tình trạng căn hộ", allowedValues);

    public static readonly Error MaCanHoNotEmpty = Error.NotEmpty("Mã căn hộ");
    public static readonly Error TenCanHoNotEmpty = Error.NotEmpty("Tên căn hộ");
    public static readonly Error MaCanHoMaxLength = Error.MaxLength("Mã căn hộ", 20);
    public static readonly Error TenCanHoMaxLength = Error.MaxLength("Tên căn hộ", 100);
    public static readonly Error DienTichRange = Error.Range("Diện tích", 1, int.MaxValue);
    public static readonly Error TangRange = Error.Range("Tầng", 1, int.MaxValue);
    public static readonly Error SoPhongNguRange = Error.Range("Số phòng ngủ", 0, int.MaxValue);
    public static readonly Error SoPhongTamRange = Error.Range("Số phòng tắm", 0, int.MaxValue);
    public static readonly Error IdRange = Error.Range("ID", 1, int.MaxValue);
}
