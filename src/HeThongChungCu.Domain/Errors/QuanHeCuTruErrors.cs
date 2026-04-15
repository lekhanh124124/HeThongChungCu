namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class QuanHeCuTruErrors
{
    public static readonly Error NotFound = new(
        "QuanHeCuTru.NotFound",
        "Không tìm thấy quan hệ cư trú.");

    public static readonly Error HouseholderAlreadyExists = new(
        "QuanHeCuTru.HouseholderAlreadyExists",
        "Căn hộ đã có chủ hộ hoặc người thuê đại diện.");

    public static Error NotFoundById(int id) => new(
        "QuanHeCuTru.NotFound",
        $"Không tìm thấy quan hệ cư trú với ID '{id}'.");

    public static Error NotFoundByIds(IEnumerable<int> ids) =>
        new(
            "QuanHeCuTru.NotFound",
            $"Không tìm thấy quan hệ cư trú với ID '{string.Join(", ", ids)}'.");

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
        "Căn hộ chưa có chủ hộ hoặc người thuê đại diện. Vui lòng thiết lập trước khi thêm thành viên khác.");

}
