namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class GeneralErrors
{
    public static readonly Error InvalidEnumValue = new(
        "General.InvalidEnumValue",
        "Giá trị liệt kê không hợp lệ.");

    public static Error BadRequest(string message) => new(
        "General.BadRequest",
        message);

    public static Error NotFoundById(int id) => new(
        "General.NotFound",
        $"Không tìm thấy bản ghi với ID '{id}'.");

    public static Error Forbidden(string message) => new(
        "General.Forbidden",
        message);
}
