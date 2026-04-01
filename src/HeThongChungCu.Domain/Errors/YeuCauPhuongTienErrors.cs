using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class YeuCauPhuongTienErrors
{
    public static readonly Error NotFound = new(
        "YeuCauPhuongTien.NotFound",
        "Không tìm thấy yêu cầu phương tiện.");

    public static readonly Error Forbidden = new(
        "YeuCauPhuongTien.Forbidden",
        "Bạn không có quyền thực hiện hành động này.");

    public static Error NotFoundById(int id) => new(
        "YeuCauPhuongTien.NotFound",
        $"Không tìm thấy yêu cầu phương tiện với ID '{id}'.");

    public static Error NotFoundByIds(List<int> ids) => new(
        "YeuCauPhuongTien.NotFound",
        $"Không tìm thấy yêu cầu phương tiện với ID '{string.Join(", ", ids)}'.");

    public static Error InvalidType(IEnumerable<string> allowedValues) => new(
        "YeuCauPhuongTien.InvalidType",
        $"Loại yêu cầu không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", allowedValues)}.");

    public static readonly Error LyDoNotEmpty = new(
        "YeuCauPhuongTien.LyDoNotEmpty",
        "Lý do không được để trống.");
}
