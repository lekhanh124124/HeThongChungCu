namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class YeuCauCuTruErrors
{
    public static readonly Error NotFound = new(
        "YeuCauCuTru.NotFound",
        "Không tìm thấy yêu cầu cư trú.");

    public static readonly Error Forbidden = new(
        "YeuCauCuTru.Forbidden",
        "Bạn không có quyền thực hiện hành động này.");

    public static Error NotFoundById(int id) => new(
        "YeuCauCuTru.NotFound",
        $"Không tìm thấy yêu cầu cư trú với ID '{id}'.");

    public static Error NotFoundByIds(List<int> ids) => new(
        "YeuCauCuTru.NotFound",
        $"Không tìm thấy yêu cầu cư trú với ID '{string.Join(", ", ids)}'.");

    public static Error InvalidType(IEnumerable<string> allowedValues) => new(
        "YeuCauCuTru.InvalidType",
        $"Loại yêu cầu không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", allowedValues)}.");

    public static readonly Error LyDoNotEmpty = new(
        "YeuCauCuTru.LyDoNotEmpty",
        "Lý do không được để trống.");

    public static Error InvalidDocumentType(IEnumerable<string> allowedValues) => new(
        "YeuCauCuTru.InvalidDocumentType",
        $"Loại giấy tờ không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", allowedValues)}.");

    public static Error InvalidRelationType(IEnumerable<string> allowedValues) => new(
        "YeuCauCuTru.InvalidRelationType",
        $"Loại quan hệ cư trú không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", allowedValues)}.");
}