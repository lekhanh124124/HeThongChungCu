using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class TangErrors
{
    public static readonly Error NotFound = new(
        "Tang.NotFound",
        "Không tìm thấy tầng với ID được chỉ định.");

    public static readonly Error MaTangAlreadyExists = new(
        "Tang.MaTangExists",
        "Đã tồn tại tầng với mã này.");

    public static readonly Error ToaNhaNotFound = new(
        "Tang.ToaNhaNotFound",
        "Không tìm thấy tòa nhà được chỉ định cho tầng này.");

    public static Error NotFoundById(int id) => new(
        "Tang.NotFound",
        $"Không tìm thấy tầng với ID '{id}'.");
}
