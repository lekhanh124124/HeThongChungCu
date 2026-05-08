using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class PhanAnhErrors
{
    public static readonly Error NotFound = new(
        "PhanAnh.NotFound",
        "Không tìm thấy yêu cầu phản ánh.");

    public static readonly Error InvalidStatus = new(
        "PhanAnh.InvalidStatus",
        "Trạng thái yêu cầu phản ánh không hợp lệ để thực hiện thao tác này.");

    public static readonly Error InvalidRating = new(
        "PhanAnh.InvalidRating",
        "Điểm đánh giá chất lượng dịch vụ phải nằm trong khoảng từ 1 đến 5 sao.");

    public static readonly Error EmptyTitleOrContent = new(
        "PhanAnh.EmptyTitleOrContent",
        "Tiêu đề hoặc nội dung phản ánh không được để trống.");

    public static readonly Error EmptyComment = new(
        "PhanAnh.EmptyComment",
        "Nội dung phản hồi không được để trống.");

    public static Error NotFoundById(int id) => new(
        "PhanAnh.NotFound",
        $"Không tìm thấy yêu cầu phản ánh với ID '{id}'.");

    public static readonly Error Forbidden = new(
        "PhanAnh.Forbidden",
        "Bạn không có quyền thực hiện thao tác trên yêu cầu phản ánh này.");
}
