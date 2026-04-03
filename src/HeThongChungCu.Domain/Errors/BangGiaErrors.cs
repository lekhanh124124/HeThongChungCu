namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class BangGiaErrors
{
    public static readonly Error NotFound = Error.NotFound("Bảng giá");

    public static Error NotFoundById(int id) => Error.NotFound("Bảng giá", id);

    public static readonly Error AlreadyExists = Error.AlreadyExists("Bảng giá");

    public static readonly Error Overlap = new(
        "BangGia.Overlap",
        "Thời gian áp dụng bảng giá bị chồng lấn với bảng giá hiện có.");

    public static Error InvalidType(IEnumerable<string> allowedValues) => 
        Error.InvalidType("Loại bảng giá", allowedValues);

    public static readonly Error LuyTienNotSupported = new(
        "BangGia.LuyTienNotSupported",
        "Bảng giá này không hỗ trợ định giá lũy tiến.");

    public static readonly Error NgayApDungNotEmpty = Error.NotEmpty("Ngày áp dụng");
    public static readonly Error NgayKetThucNotEmpty = Error.NotEmpty("Ngày kết thúc");
    public static readonly Error BangGiaIdRange = Error.Range("Bảng giá", 1, int.MaxValue);
    public static readonly Error TenBangGiaNotEmpty = Error.NotEmpty("Tên bảng giá");
    public static readonly Error TenBangGiaMaxLength = Error.MaxLength("Tên bảng giá", 200);
    public static readonly Error LuyTienNotEmpty = Error.NotEmpty("Danh sách lũy tiến");
    public static readonly Error DonGiaRange = Error.Range("Đơn giá", 0, (double)decimal.MaxValue);
    public static readonly Error TuMucNotEmpty = Error.NotEmpty("Từ số");
    public static readonly Error TuMucRange = Error.Range("Từ số", 0, (double)decimal.MaxValue);
    public static readonly Error LoaiDinhGiaNotEmpty = Error.NotEmpty("Loại định giá");
    public static readonly Error InvalidDateRange = Error.InvalidDateRange("Ngày áp dụng", "Ngày kết thúc");
}
