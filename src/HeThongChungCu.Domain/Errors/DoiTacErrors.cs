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

    // Hợp đồng đối tác
    public static readonly Error HopDongNotFound = new(
        "HopDongDoiTac.NotFound",
        "Không tìm thấy hợp đồng đối tác.");

    public static readonly Error HopDongNotActive = new(
        "HopDongDoiTac.NotActive",
        "Hợp đồng đối tác hiện đã hết hạn hoặc đã thanh lý.");

    // Hóa đơn đối tác
    public static readonly Error HoaDonNotFound = new(
        "HoaDonDoiTac.NotFound",
        "Không tìm thấy hóa đơn đối tác.");

    public static readonly Error HoaDonDuplicateKy = new(
        "HoaDonDoiTac.DuplicateKy",
        "Hợp đồng này đã lập hóa đơn đối tác cho kỳ Tháng/Năm chỉ định.");

    public static readonly Error HoaDonAlreadyPaid = new(
        "HoaDonDoiTac.AlreadyPaid",
        "Không thể chỉnh sửa hoặc xóa hóa đơn đối tác đã thanh toán.");
}
