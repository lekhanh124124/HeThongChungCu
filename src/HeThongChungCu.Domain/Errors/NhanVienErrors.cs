using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class NhanVienErrors
{
    public static readonly Error NotFound = new(
        "NhanVien.NotFound",
        "Không tìm thấy nhân viên.");
    
    public static Error NotFoundById(int id) => new(
        "NhanVien.NotFound",
        $"Không tìm thấy nhân viên với ID '{id}'.");

    public static Error NotFoundByIds(IEnumerable<int> ids) => new(
        "NhanVien.NotFound",
        $"Không tìm thấy nhân viên với ID '{string.Join(", ", ids)}'.");

    public static Error LoaiNhanVienInvalid(IEnumerable<string> allowedValues) => new(
        "Validation.InvalidType",
        $"Loại nhân viên không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", allowedValues)}.");
}
