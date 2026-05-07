using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class GiaoDichErrors
{
    public static readonly Error InvalidAmount = new(
        "GiaoDich.InvalidAmount",
        "Số tiền giao dịch phải lớn hơn 0.");

    public static readonly Error DetailAlreadyPaid = new(
        "GiaoDich.DetailAlreadyPaid",
        "Có mục phí trong danh sách đã được thanh toán trước đó.");

    public static readonly Error Overpay = new(
        "GiaoDich.Overpay",
        "Số tiền thanh toán vượt quá tổng tiền còn nợ của hóa đơn.");

    public static readonly Error PhienThanhToanNotFound = new(
        "GiaoDich.PhienThanhToanNotFound",
        "Phiên thanh toán không tồn tại.");

    public static readonly Error PhienThanhToanInvalidStatus = new(
        "GiaoDich.PhienThanhToanInvalidStatus",
        "Trạng thái phiên thanh toán không hợp lệ.");
}
