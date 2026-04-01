namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class DichVuErrors
{
    public static readonly Error NotFound = new(
        "DichVu.NotFound",
        "Không tìm thấy dịch vụ.");

    public static Error NotFoundById(int id) => new(
        "DichVu.NotFound",
        $"Không tìm thấy dịch vụ với ID '{id}'.");

    public static readonly Error AlreadyExists = new(
        "DichVu.AlreadyExists",
        "Dịch vụ đã tồn tại.");

    public static readonly Error MaDichVuAlreadyExists = new(
        "DichVu.MaDichVuAlreadyExists",
        "Mã dịch vụ đã tồn tại.");
        
    public static Error InvalidType(IEnumerable<string> allowedValues) => new(
        "DichVu.InvalidType",
        $"Loại dịch vụ không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", allowedValues)}.");
}
