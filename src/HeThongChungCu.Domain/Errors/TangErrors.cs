using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class TangErrors
{
    public static readonly Error NotFound = Error.NotFound("Tầng");

    public static readonly Error MaTangAlreadyExists = Error.AlreadyExists("tầng", "mã tầng", "");

    public static readonly Error ToaNhaNotFound = Error.NotFound("Tòa nhà");

    public static Error NotFoundById(int id) => Error.NotFound("Tầng", id);

    public static Error NotFoundByIds(IEnumerable<int> ids) => 
        Error.NotFound("Tầng", string.Join(", ", ids));

    public static Error InvalidType(IEnumerable<string> allowedValues) => 
        Error.InvalidType("Loại tầng", allowedValues);

    public static readonly Error TenTangNotEmpty = Error.NotEmpty("Tên tầng");
    public static readonly Error TenTangMaxLength = Error.MaxLength("Tên tầng", 100);
    public static readonly Error MaTangNotEmpty = Error.NotEmpty("Mã tầng");
    public static readonly Error MaTangMaxLength = Error.MaxLength("Mã tầng", 20);
    public static readonly Error TangIdRange = Error.Range("Tầng", 1, int.MaxValue);
}
