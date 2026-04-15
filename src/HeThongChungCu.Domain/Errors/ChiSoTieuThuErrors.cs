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

    public static readonly Error ChiSoCuRange = new(
        "Validation.Range",
        $"Giá trị Chỉ số cũ phải nằm trong khoảng từ 0 đến {(double)decimal.MaxValue}.");

    public static readonly Error ChiSoMoiRange = new(
        "Validation.Range",
        $"Giá trị Chỉ số mới phải nằm trong khoảng từ 0 đến {(double)decimal.MaxValue}.");

    public static readonly Error ThangRange = new(
        "Validation.Range",
        "Giá trị Tháng phải nằm trong khoảng từ 1 đến 12.");

    public static readonly Error NamRange = new(
        "Validation.Range",
        "Giá trị Năm phải nằm trong khoảng từ 2000 đến 2100.");

    public static readonly Error ChiSoTieuThuIdRange = new(
        "Validation.Range",
        $"Giá trị Chỉ số tiêu thụ phải nằm trong khoảng từ 1 đến {int.MaxValue}.");
}
