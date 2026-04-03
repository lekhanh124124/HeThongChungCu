namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class DichVuErrors
{
    public static readonly Error NotFound = Error.NotFound("Dịch vụ");

    public static Error NotFoundById(int id) => Error.NotFound("Dịch vụ", id);

    public static readonly Error AlreadyExists = Error.AlreadyExists("Dịch vụ");

    public static readonly Error MaDichVuAlreadyExists = Error.AlreadyExists("Dịch vụ", "mã dịch vụ", "");
    // Wait, the factory method for AlreadyExists with field/value expects specific field and value.
    // Error.AlreadyExists(string entity, string field, object value)
        
    public static Error InvalidType(IEnumerable<string> allowedValues) => 
        Error.InvalidType("Loại dịch vụ", allowedValues);

    public static readonly Error TenDichVuNotEmpty = Error.NotEmpty("Tên dịch vụ");
    public static readonly Error TenDichVuMaxLength = Error.MaxLength("Tên dịch vụ", 200);
    public static readonly Error MaDichVuNotEmpty = Error.NotEmpty("Mã dịch vụ");
    public static readonly Error MaDichVuMaxLength = Error.MaxLength("Mã dịch vụ", 20);
    public static readonly Error MoTaMaxLength = Error.MaxLength("Mô tả", 500);
    public static readonly Error DichVuIdRange = Error.Range("Dịch vụ", 1, int.MaxValue);
    public static readonly Error DonViTinhNotEmpty = Error.NotEmpty("Đơn vị tính");
    public static readonly Error DonViTinhMaxLength = Error.MaxLength("Đơn vị tính", 50);
}
