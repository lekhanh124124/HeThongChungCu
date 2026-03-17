using HeThongChungCu.Application.Features.ToaNha.DTOs;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.UpdateToaNha;

public record UpdateToaNhaCommand(
    int Id,
    string MaToaNha,
    string TenToaNha,
    string DiaChi,
    string? MoTa,
    int TrangThaiToaNhaId) : ICommand<ToaNhaDetailResponse>;
