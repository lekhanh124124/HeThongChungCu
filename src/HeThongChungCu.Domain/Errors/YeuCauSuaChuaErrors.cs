using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class YeuCauSuaChuaErrors
{
    public static readonly Error NotFound = new(
        "YeuCauSuaChua.NotFound",
        "Không tìm thấy yêu cầu sửa chữa.");

    public static readonly Error Forbidden = new(
        "YeuCauSuaChua.Forbidden",
        "Bạn không có quyền thực hiện hành động này.");

    public static Error NotFoundById(int id) => new(
        "YeuCauSuaChua.NotFound",
        $"Không tìm thấy yêu cầu sửa chữa với ID '{id}'.");

    public static Error NotFoundByIds(List<int> ids) => new(
        "YeuCauSuaChua.NotFound",
        $"Không tìm thấy yêu cầu sửa chữa với ID '{string.Join(", ", ids)}'.");
}