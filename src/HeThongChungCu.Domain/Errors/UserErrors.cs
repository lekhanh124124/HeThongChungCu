namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class UserErrors
{
    public static readonly Error NotFound = new(
        "User.NotFound",
        "Không tìm thấy người dùng với ID được chỉ định.");

    public static readonly Error EmailAlreadyExists = new(
        "User.EmailExists",
        "Đã tồn tại người dùng với email này.");

    public static readonly Error UsernameAlreadyExists = new(
        "User.UsernameExists",
        "Đã tồn tại người dùng với username này.");

    public static readonly Error IdCardAlreadyExists = new(
        "User.IdCardExists",
        "Đã tồn tại người dùng với CCCD/CMND này.");

    public static Error NotFoundById(int id) => new(
        "User.NotFound",
        $"Không tìm thấy người dùng với ID '{id}'.");

    public static Error NotFoundByUsername(string username) => new(
        "User.NotFound",
        $"Không tìm thấy người dùng với username '{username}' hoặc không có vai trò Cư dân/Khách.");
}
