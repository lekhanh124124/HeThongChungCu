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
}
