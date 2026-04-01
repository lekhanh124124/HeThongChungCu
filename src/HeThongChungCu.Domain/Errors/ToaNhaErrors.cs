namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class ToaNhaErrors
{
    public static readonly Error NotFound = new(
        "ToaNha.NotFound",
        "Tòa nhà không tồn tại.");

    public static readonly Error MaToaNhaAlreadyExists = new(
        "ToaNha.MaToaNhaExists",
        "Đã tồn tại tòa nhà với mã này.");

    public static Error NotFoundById(int id) => new(
        "ToaNha.NotFound",
        $"Không tìm thấy tòa nhà với ID '{id}'.");

    public static Error NotFoundByIds(IEnumerable<int> ids) => new(
        "ToaNha.NotFoundByIds",
        $"Không tìm thấy tòa nhà với các ID: {string.Join(", ", ids)}.");

    public static Error InvalidStatus(IEnumerable<string> allowedValues) => new(
        "ToaNha.InvalidStatus",
        $"Trạng thái tòa nhà không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", allowedValues)}.");
}
