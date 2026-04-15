using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class FileErrors
{
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
}
