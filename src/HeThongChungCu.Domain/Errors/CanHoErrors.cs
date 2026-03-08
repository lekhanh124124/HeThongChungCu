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

    public static Error NotFoundById(int id) => new(
        "CanHo.NotFound",
        $"Không tìm thấy căn hộ với ID '{id}'.");
}
