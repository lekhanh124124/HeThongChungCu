using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class HoaDonErrors
{
    public static readonly Error NotFound = new(
        "HoaDon.NotFound",
        "Hóa đơn không tồn tại.");

    public static readonly Error MaHoaDonRequired = new(
        "HoaDon.MaHoaDonRequired",
        "Mã hóa đơn không được để trống.");

    public static readonly Error InvalidBillingPeriod = new(
        "HoaDon.InvalidBillingPeriod",
        "Tháng thanh toán phải từ 1 đến 12.");

    public static readonly Error InvalidDueDate = new(
        "HoaDon.InvalidDueDate",
        "Ngày hạn thanh toán không thể trước ngày lập hóa đơn.");

    public static readonly Error TenMucPhiRequired = new(
        "HoaDon.TenMucPhiRequired",
        "Tên mục phí không được để trống.");

    public static readonly Error InvalidQuantity = new(
        "HoaDon.InvalidQuantity",
        "Số lượng phải lớn hơn 0.");

    public static readonly Error CannotModifyIssuedInvoice = new(
        "HoaDon.CannotModifyIssuedInvoice",
        "Không thể chỉnh sửa hóa đơn đã phát hành.");

    public static readonly Error InvalidStatusTransition = new(
        "HoaDon.InvalidStatusTransition",
        "Trạng thái chuyển đổi không hợp lệ.");

    public static readonly Error InvoiceHasNoDetails = new(
        "HoaDon.InvoiceHasNoDetails",
        "Hóa đơn không có chi tiết dịch vụ.");

    public static readonly Error InvalidBatchStatus = new(
        "HoaDon.InvalidBatchStatus",
        "Trạng thái đợt thanh toán không hợp lệ để phát hành.");

    public static readonly Error ChiTietNotFound = new(
        "HoaDon.ChiTietNotFound",
        "Chi tiết hóa đơn không tồn tại.");

    public static readonly Error InvalidPricingType = new(
        "HoaDon.InvalidPricingType",
        "Loại định giá không khớp với yêu cầu.");

    public static readonly Error InvalidAmount = new(
        "HoaDon.InvalidAmount",
        "Chi phí thực tế phải lớn hơn 0 mới có thể tạo hóa đơn.");
}
