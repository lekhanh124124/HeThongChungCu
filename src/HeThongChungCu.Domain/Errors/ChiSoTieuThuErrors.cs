namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class ChiSoTieuThuErrors
{
    public static readonly Error NotFound = new(
        "ChiSoTieuThu.NotFound",
        "Không tìm thấy chỉ số tiêu thụ.");

    public static Error NotFoundById(int id) => new(
        "ChiSoTieuThu.NotFound",
        $"Không tìm thấy chỉ số tiêu thụ với ID '{id}'.");

    public static readonly Error AlreadyExists = new(
        "ChiSoTieuThu.AlreadyExists",
        "Chỉ số tiêu thụ cho tháng/năm này đã tồn tại.");

    public static readonly Error InvalidReading = new(
        "ChiSoTieuThu.InvalidReading",
        "Chỉ số mới phải lớn hơn hoặc bằng chỉ số cũ.");

    public static readonly Error Locked = new(
        "ChiSoTieuThu.Locked",
        "Không thể xóa chỉ số tiêu thụ đã bị khóa.");
}
