using HeThongChungCu.Application.Features.QLDoiTac.DTOs;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.UpdateDoiTac;

public record UpdateDoiTacCommand(
    int Id,
    string TenDoiTac,
    string? TenCongTy,
    string? NguoiDaiDien,
    string? SoGiayPhepKD,
    string? MaSoThue,
    string? DiaChi,
    string? SoDienThoai,
    string? Email,
    string? GhiChu) : ICommand<DoiTacDetailResponse>;
