using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class BaoTriHaTangErrors
{
    // Thiết bị
    public static readonly Error ThietBiNotFound = new(
        "ThietBi.NotFound",
        "Không tìm thấy thiết bị.");

    public static readonly Error MaThietBiAlreadyExists = new(
        "ThietBi.MaThietBiAlreadyExists",
        "Mã thiết bị đã tồn tại.");

    public static Error ThietBiNotFoundById(int id) => new(
        "ThietBi.NotFound",
        $"Không tìm thấy thiết bị với ID '{id}'.");

    // Hạng mục bảo trì
    public static readonly Error HangMucNotFound = new(
        "HangMucBaoTri.NotFound",
        "Không tìm thấy hạng mục bảo trì.");

    public static readonly Error MaHangMucAlreadyExists = new(
        "HangMucBaoTri.MaHangMucAlreadyExists",
        "Mã hạng mục bảo trì đã tồn tại.");

    public static Error HangMucNotFoundById(int id) => new(
        "HangMucBaoTri.NotFound",
        $"Không tìm thấy hạng mục bảo trì với ID '{id}'.");

    // Lịch bảo trì
    public static readonly Error LichBaoTriNotFound = new(
        "LichBaoTri.NotFound",
        "Không tìm thấy lịch bảo trì.");

    public static Error LichBaoTriNotFoundById(int id) => new(
        "LichBaoTri.NotFound",
        $"Không tìm thấy lịch bảo trì với ID '{id}'.");

    // Phiếu bảo trì
    public static readonly Error PhieuBaoTriNotFound = new(
        "PhieuBaoTri.NotFound",
        "Không tìm thấy phiếu bảo trì.");

    public static readonly Error MaPhieuAlreadyExists = new(
        "PhieuBaoTri.MaPhieuAlreadyExists",
        "Mã phiếu bảo trì đã tồn tại.");

    public static Error PhieuBaoTriNotFoundById(int id) => new(
        "PhieuBaoTri.NotFound",
        $"Không tìm thấy phiếu bảo trì với ID '{id}'.");
}
