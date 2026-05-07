using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CreatePhieuBaoTri;

public record CreatePhieuBaoTriCommand(
    string MaPhieu,
    int ThietBiId,
    int HangMucBaoTriId,
    DateTimeOffset NgayDuKien,
    int? HopDongDoiTacId,
    string? GhiChuXuLy,
    List<string>? NoiDungChecklistBanDaus,
    List<PhanCongNhanSuInput>? NhanSus) : ICommand<PhieuBaoTriDetailResponse>;

public record PhanCongNhanSuInput(
    int? NhanVienId,
    string? HoTen,
    string? SoCCCD,
    string? SoDienThoai,
    string? VaiTro);
