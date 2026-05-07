using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.PhanCongNhanSuBaoTri;

public record PhanCongNhanSuBaoTriCommand(
    int Id,
    int? HopDongDoiTacId,
    DateTimeOffset NgayDuKien,
    List<PhanCongNhanSuBaoTriInput>? NhanSus) : ICommand<PhieuBaoTriDetailResponse>;

public record PhanCongNhanSuBaoTriInput(
    int? NhanVienId,
    string? HoTen,
    string? SoCCCD,
    string? SoDienThoai,
    string? VaiTro);
