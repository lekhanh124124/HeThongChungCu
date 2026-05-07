using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class GiaoDichThanhToanErrors
{
    public static readonly Error HoaDonNotPayable = new(
        "GiaoDichThanhToan.HoaDonNotPayable",
        "Hóa đơn hiện tại không thể thanh toán.");

    public static readonly Error ChiTietHoaDonInvalid = new(
        "GiaoDichThanhToan.ChiTietHoaDonInvalid",
        "Danh sách chi tiết hóa đơn không hợp lệ.");

    public static readonly Error ChiTietHoaDonAlreadyPaid = new(
        "GiaoDichThanhToan.ChiTietHoaDonAlreadyPaid",
        "Có chi tiết hóa đơn đã được thanh toán trước đó.");

    public static readonly Error Overpaid = new(
        "GiaoDichThanhToan.Overpaid",
        "Số tiền thanh toán vượt quá số tiền cần thanh toán của hóa đơn.");
}
