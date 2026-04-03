namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class DangKyDichVuErrors
{
    public static readonly Error AlreadyActive = new(
        "DangKyDichVu.AlreadyActive",
        "Căn hộ này đã đăng ký dịch vụ này và vẫn đang hoạt động.");

    public static readonly Error NotFound = Error.NotFound("Thông tin đăng ký dịch vụ");

    public static Error BusinessError(string message) => new(
        "DangKyDichVu.BusinessError",
        message);

    public static readonly Error DangKyDichVuIdRange = Error.Range("Mã đăng ký dịch vụ", 1, int.MaxValue);
    public static readonly Error NgayKetThucNotEmpty = Error.NotEmpty("Ngày kết thúc");
}
