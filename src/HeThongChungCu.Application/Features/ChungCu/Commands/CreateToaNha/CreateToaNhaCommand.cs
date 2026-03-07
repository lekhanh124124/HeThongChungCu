using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Application.Features.ChungCu.Commands.CreateToaNha;

public record CreateToaNhaCommand(
    string MaToaNha,
    string TenToaNha,
    int SoTang) : ICommand<ToaNhaResponse>;
