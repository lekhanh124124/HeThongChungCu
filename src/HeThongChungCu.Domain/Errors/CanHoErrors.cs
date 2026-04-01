namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class CanHoErrors
{
    public static readonly Error NotFound = new(
        "CanHo.NotFound",
        "Không tìm thấy căn hộ với ID được chỉ định.");

    public static readonly Error MaCanHoAlreadyExists = new(
        "CanHo.MaCanHoExists",
        "Đã tồn tại căn hộ với mã này.");

    public static readonly Error TangKhongHopLe = new(
        "CanHo.TangKhongHopLe",
        "Tầng của căn hộ không hợp lệ so với quy mô của tòa nhà.");

    public static readonly Error CanHoInBasement = new(
        "CanHo.CanHoInBasement",
        "Không thể tạo căn hộ ở tầng hầm.");

    public static Error NotFoundById(int id) => new(
        "CanHo.NotFound",
        $"Không tìm thấy căn hộ với ID '{id}'.");

    public static Error NotFoundByIds(IEnumerable<int> ids) => new(
        "CanHo.NotFound",
        $"Không tìm thấy căn hộ với ID trong danh sách: {string.Join(", ", ids)}.");

    public static Error InvalidType(IEnumerable<string> allowedValues) => new(
        "CanHo.InvalidType",
        $"Loại căn hộ không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", allowedValues)}.");

    public static Error InvalidStatus(IEnumerable<string> allowedValues) => new(
        "CanHo.InvalidStatus",
        $"Tình trạng căn hộ không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", allowedValues)}.");
}
