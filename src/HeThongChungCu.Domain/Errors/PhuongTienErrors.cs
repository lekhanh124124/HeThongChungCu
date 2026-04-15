using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Errors;

public static class PhuongTienErrors
{
    public static readonly Error NotFound = new(
        "PhuongTien.NotFound",
        "Không tìm thấy phương tiện.");

    public static readonly Error BienSoExists = new(
        "PhuongTien.BienSoAlreadyExists",
        "Biển số đã tồn tại.");

    public static readonly Error MaTheExists = new(
        "PhuongTien.MaTheAlreadyExists",
        "Mã thẻ đã tồn tại.");

    public static Error NotFoundByIds(IEnumerable<int> ids) =>
        new(
            "PhuongTien.NotFound",
            $"Không tìm thấy phương tiện với ID '{string.Join(", ", ids)}'.");

    public static Error OverQuota(LoaiCanHo loaiCanHo, LoaiPhuongTien loaiPhuongTien, int quota) => new(
        "PhuongTien.OverQuota",
        $"Căn hộ loại {loaiCanHo.Name} đã đạt hạn mức tối đa {quota} xe cho loại {loaiPhuongTien.Name}"
    );
}
