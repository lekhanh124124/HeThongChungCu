namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class ToaNhaErrors
{
    public static readonly Error NotFound = new(
        "ToaNha.NotFound",
        "Không tìm thấy tòa nhà.");

    public static readonly Error MaToaNhaAlreadyExists = new(
        "ToaNha.MaToaNhaAlreadyExists",
        "Mã tòa nhà đã tồn tại.");

    public static Error NotFoundById(int id) => new(
        "ToaNha.NotFound",
        $"Không tìm thấy tòa nhà với ID '{id}'.");

    public static Error NotFoundByIds(IEnumerable<int> ids) => 
        new(
            "ToaNha.NotFound",
            $"Không tìm thấy tòa nhà với ID '{string.Join(", ", ids)}'.");
}
