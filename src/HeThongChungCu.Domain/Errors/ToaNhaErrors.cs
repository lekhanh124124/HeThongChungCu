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
}
