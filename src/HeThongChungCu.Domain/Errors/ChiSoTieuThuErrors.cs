namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class ChiSoTieuThuErrors
{
    public static readonly Error NotFound = new(
        "ChiSoTieuThu.NotFound",
        "Không tìm thấy chỉ số tiêu thụ.");

    public static Error NotFoundById(int id) => new(
        "ChiSoTieuThu.NotFound",
        $"Không tìm thấy chỉ số tiêu thụ với ID '{id}'.");

    public static Error NotFoundByIds(int[] ids) => new(
        "ChiSoTieuThu.NotFound",
        $"Không tìm thấy chỉ số tiêu thụ với ID '{string.Join(", ", ids)}'.");
}
