namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class YeuCauCuTruErrors
{
    public static readonly Error NotFound = new(
        "YeuCauCuTru.NotFound",
        "Không tìm thấy yêu cầu cư trú.");

    public static readonly Error Forbidden = new(
        "YeuCauCuTru.Forbidden",
        "Bạn không có quyền thực hiện hành động này.");

    public static Error NotFoundById(int id) => new(
        "YeuCauCuTru.NotFound",
        $"Không tìm thấy yêu cầu cư trú với ID '{id}'.");

    public static Error NotFoundByIds(List<int> ids) => new(
        "YeuCauCuTru.NotFound",
        $"Không tìm thấy yêu cầu cư trú với ID '{string.Join(", ", ids)}'.");
}