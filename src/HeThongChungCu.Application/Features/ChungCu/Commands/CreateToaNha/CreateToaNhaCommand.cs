using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Application.Features.ChungCu.Commands.CreateToaNha;

public record CreateToaNhaCommand(
    string MaToaNha,
    string TenToaNha,
    int SoTang,
    int SoTangHam,
    string DiaChi,
    string? MoTa,
    int TrangThaiToaNhaId) : ICommand<ToaNhaResponse>;
