namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class CanHoErrors
{
    public static readonly Error NotFound = new(
        "CanHo.NotFound",
        "Không tìm thấy căn hộ.");

    public static readonly Error MaCanHoAlreadyExists = new(
        "CanHo.MaCanHoAlreadyExists",
        "Mã căn hộ đã tồn tại.");

    public static readonly Error TangKhongHopLe = new(
        "CanHo.TangKhongHopLe",
        "Tầng của căn hộ không hợp lệ so với quy mô của tòa nhà.");

    public static readonly Error CanHoInBasement = new(
        "CanHo.CanHoInBasement",
        "Không thể tạo căn hộ ở tầng hầm.");

    public static Error NotFoundById(int id) => new(
        "CanHo.NotFound",
        $"Không tìm thấy căn hộ với ID '{id}'.");

    public static Error NotFoundByIds(IEnumerable<int> ids) => 
        new(
            "CanHo.NotFound",
            $"Không tìm thấy căn hộ với ID '{string.Join(", ", ids)}'.");
}
