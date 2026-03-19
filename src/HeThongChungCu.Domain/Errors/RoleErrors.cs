namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class RoleErrors
{
    public static readonly Error NotFound = new(
        "Role.NotFound",
        "Không tìm thấy vai trò với ID được chỉ định.");

    public static readonly Error NameAlreadyExists = new(
        "Role.NameExists",
        "Đã tồn tại vai trò với tên này.");

    public static Error NotFoundById(Guid id) => new(
        "Role.NotFound",
        $"Không tìm thấy vai trò với ID '{id}'.");

    public static Error NotFoundByIds(IEnumerable<int> ids) => new(
        "Role.NotFoundByIds",
        $"Không tìm thấy vai trò với các ID: {string.Join(", ", ids)}.");
}
