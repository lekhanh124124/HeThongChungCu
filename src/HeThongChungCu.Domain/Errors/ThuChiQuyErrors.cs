using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class ThuChiQuyErrors
{
    public static readonly Error LoaiGiaoDichInvalid = new(
        "ThuChiQuy.LoaiGiaoDichInvalid",
        "Loại giao dịch không hợp lệ.");

    public static readonly Error KhoanMucInvalid = new(
        "ThuChiQuy.KhoanMucInvalid",
        "Khoản mục không hợp lệ.");

    public static readonly Error KhoanMucMismatch = new(
        "ThuChiQuy.KhoanMucMismatch",
        "Khoản mục không khớp với loại giao dịch.");

    public static readonly Error PaymentMethodInvalid = new(
        "ThuChiQuy.PaymentMethodInvalid",
        "Phương thức thanh toán không hợp lệ.");

    public static readonly Error SoTienInvalid = new(
        "ThuChiQuy.SoTienInvalid",
        "Số tiền giao dịch không hợp lệ (phải lớn hơn 0).");

    public static readonly Error DateInFuture = new(
        "ThuChiQuy.DateInFuture",
        "Ngày giao dịch không thể ở tương lai.");
}
