namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class ChiSoTieuThuErrors
{
    public static readonly Error NotFound = Error.NotFound("Chỉ số tiêu thụ");

    public static Error NotFoundById(int id) => Error.NotFound("Chỉ số tiêu thụ", id);

    public static readonly Error AlreadyExists = new(
        "ChiSoTieuThu.AlreadyExists",
        "Chỉ số tiêu thụ cho tháng/năm này đã tồn tại.");

    public static readonly Error InvalidReading = new(
        "ChiSoTieuThu.InvalidReading",
        "Chỉ số mới phải lớn hơn hoặc bằng chỉ số cũ.");

    public static readonly Error Locked = new(
        "ChiSoTieuThu.Locked",
        "Không thể xóa chỉ số tiêu thụ đã bị khóa.");

    public static readonly Error ChiSoCuRange = Error.Range("Chỉ số cũ", 0, (double)decimal.MaxValue);
    public static readonly Error ChiSoMoiRange = Error.Range("Chỉ số mới", 0, (double)decimal.MaxValue);
    public static readonly Error ThangRange = Error.Range("Tháng", 1, 12);
    public static readonly Error NamRange = Error.Range("Năm", 2000, 2100);
    public static readonly Error ChiSoTieuThuIdRange = Error.Range("Chỉ số tiêu thụ", 1, int.MaxValue);
}
