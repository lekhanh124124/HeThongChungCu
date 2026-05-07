using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class DotThanhToanErrors
{
    public static readonly Error NotFound = new(
        "DotThanhToan.NotFound",
        "Không tìm thấy đợt thanh toán.");

    public static readonly Error AlreadyExists = new(
        "DotThanhToan.AlreadyExists",
        "Đợt thanh toán cho kỳ này đã tồn tại.");

    public static Error NotFoundById(int id) => new(
        "DotThanhToan.NotFound",
        $"Không tìm thấy đợt thanh toán với ID '{id}'.");

    public static Error NotFoundByIds(List<int> ids) => new(
        "DotThanhToan.NotFound",
        $"Không tìm thấy đợt thanh toán với ID '{string.Join(", ", ids)}'.");

    public static readonly Error CannotApprove = new(
        "DotThanhToan.CannotApprove",
        "Chỉ có thể duyệt đợt thanh toán ở trạng thái Tạo mới.");

    public static readonly Error CannotClose = new(
        "DotThanhToan.CannotClose",
        "Chỉ có thể đóng đợt thanh toán đã phát hành.");
}