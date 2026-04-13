using HeThongChungCu.Application.Features.ToaNha.DTOs;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.CreateToaNha;

public record CreateToaNhaCommand(
    string MaToaNha,
    string TenToaNha,
    string Block,
    string DiaChi,
    string? MoTa) : ICommand<ToaNhaDetailResponse>;
