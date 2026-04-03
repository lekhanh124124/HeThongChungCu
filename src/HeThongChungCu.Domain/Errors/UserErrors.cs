namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class UserErrors
{
    public static readonly Error NotFound = Error.NotFound("Người dùng");

    public static readonly Error EmailAlreadyExists = Error.AlreadyExists("người dùng", "email", "");

    public static readonly Error IdCardAlreadyExists = Error.AlreadyExists("người dùng", "CCCD/CMND", "");

    public static readonly Error PhoneNumberAlreadyExists = Error.AlreadyExists("người dùng", "số điện thoại", "");

    public static Error NotFoundById(int id) => Error.NotFound("Người dùng", id);

    public static Error NotFoundByUsername(string username) => 
        Error.NotFound("Người dùng", "username", username);

    public static Error NotFoundByIds(IEnumerable<int> ids) => 
        Error.NotFound("Người dùng", string.Join(", ", ids));

    public static Error NotFoundByIdCard(string idCard) => 
        Error.NotFound("Người dùng", "CCCD/CMND", idCard);

    public static readonly Error Forbidden = Error.Forbidden("thực hiện hành động này");

    public static Error InvalidGender(IEnumerable<string> allowedValues) => 
        Error.InvalidType("Giới tính", allowedValues);

    public static readonly Error FirstNameNotEmpty = Error.NotEmpty("Họ");
    public static readonly Error FirstNameMaxLength = Error.MaxLength("Họ", 50);
    public static readonly Error EmailNotEmpty = Error.NotEmpty("Email");
    public static readonly Error DobInFuture = Error.DateInFuture("Ngày sinh");
    public static readonly Error LastNameNotEmpty = Error.NotEmpty("Tên");
    public static readonly Error LastNameMaxLength = Error.MaxLength("Tên", 50);
    public static readonly Error PhoneNumberMaxLength = Error.MaxLength("Số điện thoại", 20);
    public static readonly Error DobNotEmpty = Error.NotEmpty("Ngày sinh");
    public static readonly Error CCCDMaxLength = Error.MaxLength("CCCD/CMND", 50);
    public static readonly Error DiaChiMaxLength = Error.MaxLength("Địa chỉ", 200);
    public static readonly Error GenderNotEmpty = Error.NotEmpty("Giới tính");
    public static readonly Error GenderRange = Error.Range("Giới tính", 1, 2);
    public static readonly Error UserIdRange = Error.Range("Người dùng", 1, int.MaxValue);
}
