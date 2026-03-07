using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Application.Features.ChungCu.Commands.UpdateToaNha;

public record UpdateToaNhaCommand(
    int Id,
    string TenToaNha,
    int SoTang) : ICommand<ToaNhaResponse>;
