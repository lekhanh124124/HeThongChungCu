using HeThongChungCu.Application.Features.QLDoiTac.DTOs;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.CreateDoiTac;

public record CreateDoiTacCommand(
    string TenDoiTac,
    string? TenCongTy,
    string? NguoiDaiDien,
    string? SoGiayPhepKD,
    string? MaSoThue,
    string? DiaChi,
    string? SoDienThoai,
    string? Email,
    string? GhiChu,
    List<HopDongRequestDto>? HopDongs) : ICommand<DoiTacDetailResponse>;
