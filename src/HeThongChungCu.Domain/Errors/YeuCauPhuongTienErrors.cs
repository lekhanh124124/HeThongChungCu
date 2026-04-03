using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class YeuCauPhuongTienErrors
{
    public static readonly Error NotFound = Error.NotFound("Yêu cầu phương tiện");

    public static readonly Error Forbidden = Error.Forbidden("thực hiện hành động này");

    public static Error NotFoundById(int id) => Error.NotFound("Yêu cầu phương tiện", id);

    public static Error NotFoundByIds(List<int> ids) => Error.NotFound("Yêu cầu phương tiện", string.Join(", ", ids));

    public static Error InvalidType(IEnumerable<string> allowedValues) => 
        Error.InvalidType("Loại yêu cầu", allowedValues);

    public static readonly Error LyDoNotEmpty = Error.NotEmpty("Lý do");
    public static readonly Error HienTrangNotEmpty = Error.NotEmpty("Hiện trạng");
    public static readonly Error GhiChuMaxLength = Error.MaxLength("Ghi chú", 500);
    public static readonly Error BienSoNotEmpty = Error.NotEmpty("Biển số");
    public static readonly Error MauXeNotEmpty = Error.NotEmpty("Màu xe");
    public static readonly Error TenXeNotEmpty = Error.NotEmpty("Tên xe");
    public static readonly Error LoaiXeIdRange = Error.Range("Loại xe", 1, int.MaxValue);
    public static readonly Error CanHoIdRange = Error.Range("Căn hộ", 1, int.MaxValue);
    public static readonly Error YeuCauPhuongTienIdRange = Error.Range("Yêu cầu phương tiện", 1, int.MaxValue);
    public static readonly Error FileIdRange = Error.Range("Tệp tin", 1, int.MaxValue);
    public static readonly Error LoaiYeuCauNotEmpty = Error.NotEmpty("Loại yêu cầu");
    public static readonly Error YeuCauPhuongTienIdsNotEmpty = Error.NotEmpty("Danh sách yêu cầu phương tiện");
}
