using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class PhuongTienErrors
{
    public static readonly Error NotFound = new(
        "PhuongTien.NotFound", 
        "Phương tiện không tồn tại.");

    public static readonly Error BienSoExists = new(
        "PhuongTien.BienSoExists", 
        "Biển số phương tiện đã tồn tại trong hệ thống.");

    public static readonly Error MaTheExists = new(
        "PhuongTien.MaTheExists", 
        "Mã thẻ phương tiện đã tồn tại trong hệ thống.");

    public static Error NotFoundByIds(IEnumerable<int> ids) => new(
        "PhuongTien.NotFoundByIds", 
        $"Không tìm thấy phương tiện với các ID: {string.Join(", ", ids)}.");

    public static Error InvalidType(IEnumerable<string> allowedValues) => new(
        "PhuongTien.InvalidType",
        $"Loại phương tiện không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", allowedValues)}.");
}
