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

    public static Error NotFoundByIds(IEnumerable<int> ids) => new(
        "DichVu.NotFound",
        $"Không tìm thấy dịch vụ với ID '{string.Join(", ", ids)}'.");

    public static readonly Error AlreadyExists = new(
        "DichVu.AlreadyExists",
        "Dịch vụ đã tồn tại.");

    public static Error MaDichVuAlreadyExists(string maDichVu) => new(
        "DichVu.MaDichVuAlreadyExists",
        $"Đã tồn tại dịch vụ với mã dịch vụ '{maDichVu}'.");

    public static Error InvalidType(IEnumerable<string> allowedValues) => new(
        "Validation.InvalidType",
        $"Loại dịch vụ không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", allowedValues)}.");

    public static readonly Error KhungGioOverlap = new(
        "DichVu.KhungGioOverlap",
        "Khung giờ mới bị trùng lặp với khung giờ đã tồn tại.");

    public static readonly Error KhungGioNotFound = new(
        "DichVu.KhungGioNotFound",
        "Không tìm thấy khung giờ của dịch vụ.");

    public static readonly Error BangGiaOverlap = new(
        "DichVu.BangGiaOverlap",
        "Thời gian áp dụng bảng giá mới bị chồng lấn với bảng giá hiện tại.");

    public static Error NotActive(string tenDichVu) => new(
        "DichVu.NotActive",
        $"Dịch vụ '{tenDichVu}' hiện đang không hoạt động.");

    public static readonly Error MissingSlotInfo = new(
        "DichVu.MissingSlotInfo",
        "Dịch vụ yêu cầu chọn khung giờ và ngày đăng ký.");

    public static readonly Error InvalidSlot = new(
        "DichVu.InvalidSlot",
        "Khung giờ không thuộc dịch vụ này.");

    public static Error InvalidDayOfWeek(string tenKhungGio) => new(
        "DichVu.InvalidDayOfWeek",
        $"Khung giờ '{tenKhungGio}' không áp dụng cho ngày được chọn.");

    public static Error CapacityExceeded(string tenDichVu, int hienTai, int toiDa) => new(
        "DichVu.CapacityExceeded",
        $"Dịch vụ '{tenDichVu}' đã đạt giới hạn sức chứa cho thời điểm/khung giờ này. (Hiện tại: {hienTai}, Tối đa: {toiDa})");

    // BangGia Related
    public static readonly Error BangGiaNotFound = new(
        "DichVu.BangGiaNotFound",
        "Không tìm thấy bảng giá.");

    public static Error BangGiaNotFoundById(int id) => new(
        "DichVu.BangGiaNotFound",
        $"Không tìm thấy bảng giá với ID '{id}'.");

    public static readonly Error LoaiDinhGiaNotSupported = new(
        "DichVu.LoaiDinhGiaNotSupported",
        "Loại hình định giá này chưa được hỗ trợ cấu hình qua API này.");

    public static readonly Error DonGiaCoDinhRequired = new(
        "DichVu.DonGiaCoDinhRequired",
        "Đơn giá không được để trống cho bảng giá cố định.");

    public static readonly Error GetBangGiaAfterActionFailed = new(
        "DichVu.GetBangGiaAfterActionFailed",
        "Đã có lỗi xảy ra khi lấy thông tin bảng giá sau khi thực hiện thao tác.");
}

