using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class TangErrors
{
    public static readonly Error NotFound = new(
        "Tang.NotFound",
        "Không tìm thấy tầng.");

    public static readonly Error MaTangAlreadyExists = new(
        "Tang.MaTangAlreadyExists",
        "Mã tầng đã tồn tại.");

    public static readonly Error ToaNhaNotFound = new(
        "ToaNha.NotFound",
        "Không tìm thấy tòa nhà.");

    public static Error NotFoundById(int id) => new(
        "Tang.NotFound",
        $"Không tìm thấy tầng với ID '{id}'.");

    public static Error NotFoundByIds(IEnumerable<int> ids) => 
        new(
            "Tang.NotFound",
            $"Không tìm thấy tầng với ID '{string.Join(", ", ids)}'.");
}
