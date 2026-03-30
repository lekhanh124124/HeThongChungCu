namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class UserErrors
{
    public static readonly Error NotFound = new(
        "User.NotFound",
        "Không tìm thấy thông tin người dùng.");

    public static readonly Error EmailAlreadyExists = new(
        "User.EmailExists",
        "Đã tồn tại người dùng với email này.");

    public static readonly Error IdCardAlreadyExists = new(
        "User.IdCardExists",
        "Đã tồn tại người dùng với CCCD/CMND này.");

    public static readonly Error PhoneNumberAlreadyExists = new(
        "User.PhoneNumberExists",
        "Đã tồn tại người dùng với số điện thoại này.");


    public static Error NotFoundById(int id) => new(
        "User.NotFound",
        $"Không tìm thấy người dùng với ID '{id}'.");

    public static Error NotFoundByUsername(string username) => new(
        "User.NotFound",
        $"Không tìm thấy người dùng với username '{username}'.");

    public static Error NotFoundByIds(IEnumerable<int> ids) => new(
        "User.NotFound",
        $"Không tìm thấy người dùng với các ID: {string.Join(", ", ids)}.");

    public static Error NotFoundByIdCard(string idCard) => new(
        "User.NotFound",
        $"Không tìm thấy người dùng với CCCD/CMND '{idCard}'.");
}
