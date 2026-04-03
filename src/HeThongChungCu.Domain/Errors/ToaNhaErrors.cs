namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class ToaNhaErrors
{
    public static readonly Error NotFound = Error.NotFound("Tòa nhà");

    public static readonly Error MaToaNhaAlreadyExists = Error.AlreadyExists("tòa nhà", "mã tòa nhà", "");

    public static Error NotFoundById(int id) => Error.NotFound("Tòa nhà", id);

    public static Error NotFoundByIds(IEnumerable<int> ids) => 
        Error.NotFound("Tòa nhà", string.Join(", ", ids));

    public static Error InvalidStatus(IEnumerable<string> allowedValues) => 
        Error.InvalidType("Trạng thái tòa nhà", allowedValues);

    public static readonly Error TenToaNhaNotEmpty = Error.NotEmpty("Tên tòa nhà");
    public static readonly Error TenToaNhaMaxLength = Error.MaxLength("Tên tòa nhà", 100);
    public static readonly Error MaToaNhaNotEmpty = Error.NotEmpty("Mã tòa nhà");
    public static readonly Error MaToaNhaMaxLength = Error.MaxLength("Mã tòa nhà", 20);
    public static readonly Error ToaNhaIdRange = Error.Range("Tòa nhà", 1, int.MaxValue);
    public static readonly Error DiaChiNotEmpty = Error.NotEmpty("Địa chỉ");
    public static readonly Error DiaChiMaxLength = Error.MaxLength("Địa chỉ", 255);
}
