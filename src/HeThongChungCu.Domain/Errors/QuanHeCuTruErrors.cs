namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class QuanHeCuTruErrors
{
    public static readonly Error NotFound = new(
        "QuanHeCuTru.NotFound",
        "Không tìm thấy quan hệ cư trú với ID được chỉ định.");

    public static Error NotFoundById(int id) => new(
        "QuanHeCuTru.NotFound",
        $"Không tìm thấy quan hệ cư trú với ID '{id}'.");

    public static readonly Error UserAlreadyResident = new(
        "QuanHeCuTru.UserAlreadyResident",
        "Cư dân này đã đang cư trú tại căn hộ.");

    public static readonly Error CuTruDaKetThuc = new(
        "QuanHeCuTru.CuTruDaKetThuc",
        "Quan hệ cư trú này đã kết thúc.");

    public static readonly Error LoaiQuanHeKhongHopLe = new(
        "QuanHeCuTru.LoaiQuanHeKhongHopLe",
        "Loại quan hệ cư trú không hợp lệ.");
}
