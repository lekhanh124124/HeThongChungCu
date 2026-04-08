using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class DoiTacErrors
{
    public static readonly Error NotFound = Error.NotFound("Đơn vị cung cấp");

    public static Error NotFoundById(int id) => Error.NotFound("Đơn vị cung cấp", id);

    public static readonly Error TenDoiTacNotEmpty = Error.NotEmpty("Tên đơn vị cung cấp");
    public static readonly Error TenDoiTacMaxLength = Error.MaxLength("Tên đơn vị cung cấp", 100);
    
    public static readonly Error TenCongTyNotEmpty = Error.NotEmpty("Tên công ty");
    public static readonly Error TenCongTyMaxLength = Error.MaxLength("Tên công ty", 200);

    public static readonly Error NguoiDaiDienMaxLength = Error.MaxLength("Người đại diện", 100);
    public static readonly Error SoGiayPhepKDMaxLength = Error.MaxLength("Số giấy phép kinh doanh", 50);
    public static readonly Error MaSoThueMaxLength = Error.MaxLength("Mã số thuế", 50);
    public static readonly Error GhiChuMaxLength = Error.MaxLength("Ghi chú", 1000);

    public static readonly Error SoDienThoaiMaxLength = Error.MaxLength("Số điện thoại", 20);
    public static readonly Error EmailMaxLength = Error.MaxLength("Email", 100);

    public static readonly Error IdNotEmpty = Error.NotEmpty("ID");
    public static readonly Error HopDongsNotEmpty = new("DoiTac.HopDongsNotEmpty", "Phải có ít nhất một hợp đồng cung cấp dịch vụ.");
    
    public static readonly Error SoHopDongNotEmpty = Error.NotEmpty("Số hợp đồng");
    public static readonly Error NgayHetHanInvalid = new("DoiTac.NgayHetHanInvalid", "Ngày hết hạn phải sau ngày ký.");
    public static readonly Error GiaTriHopDongNegative = new("DoiTac.GiaTriHopDongNegative", "Giá trị hợp đồng không được âm.");

    public static readonly Error MaDichVuNotEmpty = Error.NotEmpty("Mã dịch vụ");
    public static readonly Error TenDichVuNotEmpty = Error.NotEmpty("Tên dịch vụ");
    public static readonly Error LoaiDichVuInvalid = new("DoiTac.LoaiDichVuInvalid", "Loại dịch vụ không hợp lệ.");
    public static readonly Error DonViTinhNotEmpty = Error.NotEmpty("Đơn vị tính");
    public static readonly Error SoLuongToiDaInvalid = new("DoiTac.SoLuongToiDaInvalid", "Số lượng tối đa phải lớn hơn 0.");
}
