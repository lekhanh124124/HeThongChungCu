namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class BangGiaErrors
{
    public static readonly Error NotFound = new(
        "BangGia.NotFound",
        "Không tìm thấy bảng giá.");

    public static Error NotFoundById(int id) => new(
        "BangGia.NotFound",
        $"Không tìm thấy bảng giá với ID '{id}'.");

    public static readonly Error AlreadyExists = new(
        "BangGia.AlreadyExists",
        "Bảng giá đã tồn tại.");

    public static readonly Error Overlap = new(
        "BangGia.Overlap",
        "Thời gian áp dụng bảng giá bị chồng lấn với bảng giá hiện có.");

    public static Error InvalidType(IEnumerable<string> allowedValues) => new(
        "BangGia.InvalidType",
        $"Loại bảng giá không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", allowedValues)}.");

    public static readonly Error LuyTienNotSupported = new(
        "BangGia.LuyTienNotSupported",
        "Bảng giá này không hỗ trợ định giá lũy tiến.");
}
