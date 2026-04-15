namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class RoleErrors
{
    public static readonly Error NotFound = new(
        "Role.NotFound",
        "Không tìm thấy vai trò.");

    public static readonly Error NameAlreadyExists = new(
        "Role.NameAlreadyExists",
        "Tên vai trò đã tồn tại.");

    public static Error NotFoundById(Guid id) => new(
        "Role.NotFound",
        $"Không tìm thấy vai trò với ID '{id}'.");

    public static Error NotFoundByIds(IEnumerable<int> ids) => 
        new(
            "Role.NotFound",
            $"Không tìm thấy vai trò với ID '{string.Join(", ", ids)}'.");

    public static readonly Error NameNotEmpty = new(
        "Validation.NotEmpty",
        "Tên vai trò không được để trống.");

    public static readonly Error NameMaxLength = new(
        "Validation.MaxLength",
        "Tên vai trò không được vượt quá 50 ký tự.");

    public static readonly Error DescriptionMaxLength = new(
        "Validation.MaxLength",
        "Mô tả không được vượt quá 200 ký tự.");

    public static readonly Error RoleIdRange = new(
        "Validation.Range",
        $"Giá trị Vai trò phải nằm trong khoảng từ 1 đến {int.MaxValue}.");
}
