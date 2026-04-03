using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Errors;

public static class PhuongTienErrors
{
    public static readonly Error NotFound = Error.NotFound("Phương tiện");

    public static readonly Error BienSoExists = Error.AlreadyExists("phương tiện", "biển số", "");

    public static readonly Error MaTheExists = Error.AlreadyExists("thẻ phương tiện", "mã thẻ", "");

    public static Error NotFoundByIds(IEnumerable<int> ids) =>
        Error.NotFound("Phương tiện", string.Join(", ", ids));

    public static Error InvalidType(IEnumerable<string> allowedValues) =>
        Error.InvalidType("Loại phương tiện", allowedValues);

    public static Error OverQuota(LoaiCanHo loaiCanHo, LoaiPhuongTien loaiPhuongTien, int quota) => new(
        "PhuongTien.OverQuota",
        $"Căn hộ loại {loaiCanHo.Name} đã đạt hạn mức tối đa {quota} xe cho loại {loaiPhuongTien.Name}"
    );

    public static readonly Error BienSoNotEmpty = Error.NotEmpty("Biển số");
    public static readonly Error BienSoMaxLength = Error.MaxLength("Biển số", 20);
    public static readonly Error MauXeNotEmpty = Error.NotEmpty("Màu xe");
    public static readonly Error MauXeMaxLength = Error.MaxLength("Màu xe", 50);
    public static readonly Error TenXeNotEmpty = Error.NotEmpty("Tên xe");
    public static readonly Error TenXeMaxLength = Error.MaxLength("Tên xe", 100);
    public static readonly Error LoaiXeIdRange = Error.Range("Loại xe", 1, int.MaxValue);
    public static readonly Error ChuSoHuuIdRange = Error.Range("Chủ sở hữu", 1, int.MaxValue);
    public static readonly Error PhuongTienIdRange = Error.Range("Phương tiện", 1, int.MaxValue);
    public static readonly Error MaTheNotEmpty = Error.NotEmpty("Mã thẻ");
    public static readonly Error MaTheMaxLength = Error.MaxLength("Mã thẻ", 50);
    public static readonly Error TheIdRange = Error.Range("Thẻ phương tiện", 1, int.MaxValue);
    public static readonly Error PhuongTienIdsNotEmpty = Error.NotEmpty("Danh sách phương tiện");
    public static readonly Error TheIdsNotEmpty = Error.NotEmpty("Danh sách thẻ phương tiện");
}
