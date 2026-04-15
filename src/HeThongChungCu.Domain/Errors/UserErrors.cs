namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class UserErrors
{
    public static readonly Error NotFound = new(
        "NguoiDung.NotFound",
        "Không tìm thấy người dùng.");

    public static readonly Error EmailAlreadyExists = new(
        "NguoiDung.EmailAlreadyExists",
        "Email đã tồn tại.");

    public static readonly Error IdCardAlreadyExists = new(
        "NguoiDung.IdCardAlreadyExists",
        "CCCD/CMND đã tồn tại.");

    public static readonly Error PhoneNumberAlreadyExists = new(
        "NguoiDung.PhoneNumberAlreadyExists",
        "Số điện thoại đã tồn tại.");

    public static Error NotFoundById(int id) => new(
        "NguoiDung.NotFound",
        $"Không tìm thấy người dùng với ID '{id}'.");

    public static Error NotFoundByIdCard(string idCard) => new(
        "NguoiDung.NotFound",
        $"Không tìm thấy người dùng với CCCD/CMND '{idCard}'.");

    public static Error InvalidGender(IEnumerable<string> allowedValues) => new(
        "Validation.InvalidType",
        $"Giới tính không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", allowedValues)}.");

    public static readonly Error AccountNotFound = new(
        "TaiKhoan.NotFound",
        "Không tìm thấy tài khoản.");
}
