using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class DoiTacErrors
{
    public static readonly Error NotFound = new(
        "DoiTac.NotFound",
        "Không tìm thấy đơn vị cung cấp.");

    public static Error NotFoundById(int id) => new(
        "DoiTac.NotFound",
        $"Không tìm thấy đơn vị cung cấp với ID '{id}'.");

    public static readonly Error InvalidLoaiDichVu = new(
        "DoiTac.InvalidLoaiDichVu",
        "Loại dịch vụ không hợp lệ.");
}
