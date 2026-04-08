namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class DichVuErrors
{
    public static readonly Error NotFound = Error.NotFound("Dịch vụ");
    public static Error NotFoundById(int id) => Error.NotFound("Dịch vụ", id);
    public static Error NotFoundByIds(IEnumerable<int> ids) => Error.NotFound("Dịch vụ", string.Join(", ", ids));
    public static readonly Error AlreadyExists = Error.AlreadyExists("Dịch vụ");

    public static Error MaDichVuAlreadyExists(string maDichVu) => 
        Error.AlreadyExists("Dịch vụ", "mã dịch vụ", maDichVu);

    public static Error InvalidType(IEnumerable<string> allowedValues) => 
        Error.InvalidType("Loại dịch vụ", allowedValues);

    public static readonly Error TenDichVuNotEmpty = Error.NotEmpty("Tên dịch vụ");
    public static readonly Error TenDichVuMaxLength = Error.MaxLength("Tên dịch vụ", 200);
    public static readonly Error MaDichVuNotEmpty = Error.NotEmpty("Mã dịch vụ");
    public static readonly Error MaDichVuMaxLength = Error.MaxLength("Mã dịch vụ", 20);
    public static readonly Error MoTaMaxLength = Error.MaxLength("Mô tả", 500);
    public static readonly Error DichVuIdRange = Error.Range("Dịch vụ", 1, int.MaxValue);
    public static readonly Error DonViTinhNotEmpty = Error.NotEmpty("Đơn vị tính");
    public static readonly Error DonViTinhMaxLength = Error.MaxLength("Đơn vị tính", 50);

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

    public static readonly Error CanHoIdNotEmpty = Error.NotEmpty("Mã căn hộ");
    public static readonly Error SoLuongPositive = Error.Range("Số lượng", 1, int.MaxValue);
    public static readonly Error NgaySuDungNotEmpty = Error.NotEmpty("Ngày sử dụng");

    // BangGia Related
    public static readonly Error BangGiaNotFound = Error.NotFound("Bảng giá");
    public static Error BangGiaNotFoundById(int id) => Error.NotFound("Bảng giá", id);

    public static readonly Error LoaiDinhGiaNotSupported = new(
        "DichVu.LoaiDinhGiaNotSupported",
        "Loại hình định giá này chưa được hỗ trợ cấu hình qua API này.");

    public static readonly Error DonGiaCoDinhRequired = new(
        "DichVu.DonGiaCoDinhRequired",
        "Đơn giá không được để trống cho bảng giá cố định.");

    public static readonly Error GetBangGiaAfterActionFailed = new(
        "DichVu.GetBangGiaAfterActionFailed",
        "Đã có lỗi xảy ra khi lấy thông tin bảng giá sau khi thực hiện thao tác.");

    // Validation Messages (Consolidated here as requested)
    public static readonly Error TenBangGiaNotEmpty = Error.NotEmpty("Tên bảng giá");
    public static readonly Error TenBangGiaMaxLength = Error.MaxLength("Tên bảng giá", 100);
    public static readonly Error NgayApDungNotEmpty = Error.NotEmpty("Ngày áp dụng");
    public static readonly Error LoaiDinhGiaInvalid = new("Validation.LoaiDinhGiaInvalid", "Loại định giá không hợp lệ.");
    public static readonly Error GiaLuyTienNotEmpty = new("Validation.GiaLuyTienNotEmpty", "Bảng giá lũy tiến phải có ít nhất một bậc giá.");
    public static readonly Error GiaKhungGioNotEmpty = new("Validation.GiaKhungGioNotEmpty", "Bảng giá khung giờ phải có ít nhất một thông tin giá.");
    public static readonly Error GiaLoaiCanHoNotEmpty = new("Validation.GiaLoaiCanHoNotEmpty", "Bảng giá theo loại căn hộ phải có ít nhất một thông tin giá.");
    public static readonly Error BangGiaOverlapEx = new("Validation.BangGiaOverlap", "Thời gian áp dụng bảng giá mới bị chồng lấn với bảng giá khác hiện có.");
    public static readonly Error CoDinhNoLuyTien = new("Validation.CoDinhNoLuyTien", "Bảng giá cố định không được có chi tiết lũy tiến.");
    public static readonly Error CoDinhNoKhungGio = new("Validation.CoDinhNoKhungGio", "Bảng giá cố định không được có chi tiết khung giờ.");
    public static readonly Error CoDinhNoLoaiCanHo = new("Validation.CoDinhNoLoaiCanHo", "Bảng giá cố định không được có chi tiết loại căn hộ.");
    public static readonly Error LuyTienNoDonGia = new("Validation.LuyTienNoDonGia", "Bảng giá lũy tiến không dùng đơn giá cố định.");
    public static readonly Error LuyTienNoKhungGio = new("Validation.LuyTienNoKhungGio", "Bảng giá lũy tiến không được có chi tiết khung giờ.");
    public static readonly Error LuyTienNoLoaiCanHo = new("Validation.LuyTienNoLoaiCanHo", "Bảng giá lũy tiến không được có chi tiết loại căn hộ.");
    public static readonly Error KhungGioNoDonGia = new("Validation.KhungGioNoDonGia", "Bảng giá khung giờ không dùng đơn giá cố định.");
    public static readonly Error KhungGioNoLuyTien = new("Validation.KhungGioNoLuyTien", "Bảng giá khung giờ không được có chi tiết lũy tiến.");
    public static readonly Error KhungGioNoLoaiCanHo = new("Validation.KhungGioNoLoaiCanHo", "Bảng giá khung giờ không được có chi tiết loại căn hộ.");
    public static readonly Error DienTichNoDonGia = new("Validation.DienTichNoDonGia", "Bảng giá theo diện tích không dùng đơn giá cố định.");
    public static readonly Error DienTichNoLuyTien = new("Validation.DienTichNoLuyTien", "Bảng giá theo diện tích không được có chi tiết lũy tiến.");
    public static readonly Error DienTichNoKhungGio = new("Validation.DienTichNoKhungGio", "Bảng giá theo diện tích không được có chi tiết khung giờ.");
    public static readonly Error DonGiaPositive = new("Validation.DonGiaPositive", "Đơn giá phải >= 0.");
    public static readonly Error TuMucRange = new("Validation.TuMucRange", "Từ số phải >= 0.");
    public static readonly Error NgayKetThucGreaterThanBatDau = new("Validation.NgayKetThucGreaterThanBatDau", "Ngày kết thúc phải lớn hơn ngày bắt đầu.");

    // KhungGio Related
    public static readonly Error IdKhungGioNotEmpty = Error.NotEmpty("ID khung giờ");
    public static readonly Error TenKhungGioNotEmpty = Error.NotEmpty("Tên khung giờ");
    public static readonly Error TenKhungGioMaxLength = Error.MaxLength("Tên khung giờ", 100);
    public static readonly Error GioBatDauNotEmpty = Error.NotEmpty("Giờ bắt đầu");
    public static readonly Error GioKetThucNotEmpty = Error.NotEmpty("Giờ kết thúc");
    public static readonly Error GioKetThucGreaterThanBatDau = new("Validation.GioKetThucGreaterThanBatDau", "Giờ kết thúc phải lớn hơn giờ bắt đầu.");
    public static readonly Error NgayTrongTuanRange = new("Validation.NgayTrongTuan", "Ngày trong tuần phải từ 0 (Chủ nhật) đến 6 (Thứ bảy).");
}

