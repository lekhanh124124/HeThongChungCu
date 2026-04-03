namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class RoleErrors
{
    public static readonly Error NotFound = Error.NotFound("Vai trò");

    public static readonly Error NameAlreadyExists = Error.AlreadyExists("vai trò", "tên", "");

    public static Error NotFoundById(Guid id) => Error.NotFound("Vai trò", id);

    public static Error NotFoundByIds(IEnumerable<int> ids) => 
        Error.NotFound("Vai trò", string.Join(", ", ids));

    public static readonly Error NameNotEmpty = Error.NotEmpty("Tên vai trò");
    public static readonly Error NameMaxLength = Error.MaxLength("Tên vai trò", 50);
    public static readonly Error DescriptionMaxLength = Error.MaxLength("Mô tả", 200);
    public static readonly Error RoleIdRange = Error.Range("Vai trò", 1, int.MaxValue);
}
