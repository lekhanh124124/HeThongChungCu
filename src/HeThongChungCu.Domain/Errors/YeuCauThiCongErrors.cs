using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class YeuCauThiCongErrors
{
    public static readonly Error NotFound = new(
        "YeuCauThiCong.NotFound",
        "Không tìm thấy yêu cầu sửa chữa.");

    public static readonly Error Forbidden = new(
        "YeuCauThiCong.Forbidden",
        "Bạn không có quyền thực hiện hành động này.");

    public static readonly Error HuyForbidden = new(
        "YeuCauThiCong.Forbidden",
        "Yêu cầu đã được xác nhận hoặc đang triển khai, vui lòng liên hệ BQL để được hỗ trợ hủy trực tiếp.");

    public static Error NotFoundById(int id) => new(
        "YeuCauThiCong.NotFound",
        $"Không tìm thấy yêu cầu sửa chữa với ID '{id}'.");

    public static Error NotFoundByIds(List<int> ids) => new(
        "YeuCauThiCong.NotFound",
        $"Không tìm thấy yêu cầu sửa chữa với ID '{string.Join(", ", ids)}'.");

    public static Error NotBelongToUser(int userId, int yeuCauId) => new(
        "YeuCauThiCong.NotBelongToUser",
        $"Yêu cầu sửa chữa ID '{yeuCauId}' không thuộc về người dùng ID '{userId}'.");
}
