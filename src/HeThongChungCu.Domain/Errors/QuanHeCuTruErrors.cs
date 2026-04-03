namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class QuanHeCuTruErrors
{
    public static readonly Error NotFound = Error.NotFound("Quan hệ cư trú");

    public static Error NotFoundById(int id) => Error.NotFound("Quan hệ cư trú", id);

    public static Error NotFoundByIds(IEnumerable<int> ids) => 
        Error.NotFound("Quan hệ cư trú", string.Join(", ", ids));

    public static readonly Error UserAlreadyResident = new(
        "QuanHeCuTru.UserAlreadyResident",
        "Cư dân này đã đang cư trú tại căn hộ.");

    public static readonly Error CuTruDaKetThuc = new(
        "QuanHeCuTru.CuTruDaKetThuc",
        "Quan hệ cư trú này đã kết thúc.");

    public static readonly Error LoaiQuanHeKhongHopLe = new(
        "QuanHeCuTru.LoaiQuanHeKhongHopLe",
        "Loại quan hệ cư trú không hợp lệ.");

    public static readonly Error HouseholderNotFound = new(
        "QuanHeCuTru.HouseholderNotFound",
        "Căn hộ chưa có chủ hộ. Vui lòng thiết lập chủ hộ trước khi thêm thành viên khác.");

}
