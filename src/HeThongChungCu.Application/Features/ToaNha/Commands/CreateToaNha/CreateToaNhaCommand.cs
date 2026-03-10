using HeThongChungCu.Application.Features.ToaNha.DTOs;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.CreateToaNha;

public record CreateToaNhaCommand(
    string MaToaNha,
    string TenToaNha,
    int SoTang,
    int SoTangHam,
    string DiaChi,
    string? MoTa) : ICommand<ToaNhaDetailResponse>;
