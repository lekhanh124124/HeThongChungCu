using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class FileErrors
{
    public static readonly Error DuplicateFileName = new(
        "File.DuplicateFileName",
        "Trong một lượt tải lên không được có các tệp tin trùng tên nhau để đảm bảo việc ánh xạ dữ liệu chính xác.");

    public static Error TooLarge(int maxSizeMB) => new(
        "File.TooLarge",
        $"Dung lượng tệp tin vượt quá giới hạn cho phép ({maxSizeMB}MB).");

    public static Error InvalidType(string extension, string[] allowedExtensions) => new(
        "File.InvalidType",
        $"Tệp tin '{extension}' không hợp lệ. Chỉ chấp nhận: {string.Join(", ", allowedExtensions)}.");

    public static readonly Error SignatureMismatch = new(
        "File.SignatureMismatch",
        "Tệp tin không đúng định dạng kỹ thuật (có thể tệp đã bị đổi tên sai cách). Vui lòng kiểm tra lại tệp tin gốc.");
    
    public static readonly Error UnrecognizedCategory = new(
        "File.UnrecognizedCategory",
        "Loại tệp tin không được hệ thống hỗ trợ.");

    public static readonly Error EmptyTargetContainer = new(
        "File.EmptyTargetContainer",
        "Mục đích tải lên (Target Container/Category) không được để trống.");

    public static readonly Error EmptyFileName = new(
        "File.EmptyFileName",
        "Tên tệp tin không được để trống.");

    public static readonly Error EmptyContent = new(
        "File.EmptyContent",
        "Nội dung tệp tin không được để trống.");

    public static readonly Error InvalidSize = new(
        "File.InvalidSize",
        "Tệp tin không được rỗng.");

    public static Error InvalidContentType(string[] allowedTypes) => new(
        "File.InvalidContentType",
        $"Chỉ cho phép định dạng: {string.Join(", ", allowedTypes)}.");
}
