using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class NhanVienErrors
{
    public static readonly Error NotFound = Error.NotFound("Nhân viên");
    
    public static readonly Error MaNhanVienAlreadyExists = Error.AlreadyExists("nhân viên", "mã nhân viên", "");

    public static Error NotFoundById(int id) => Error.NotFound("Nhân viên", id);
    public static Error NotFoundByIds(IEnumerable<int> ids) => Error.NotFound("Nhân viên", ids);

    public static readonly Error MaNhanVienNotEmpty = Error.NotEmpty("Mã nhân viên");
    public static readonly Error MaNhanVienMaxLength = Error.MaxLength("Mã nhân viên", 20);
    public static Error LoaiNhanVienInvalid(IEnumerable<string> allowedValues) => Error.InvalidType("Loại nhân viên", allowedValues);
}
