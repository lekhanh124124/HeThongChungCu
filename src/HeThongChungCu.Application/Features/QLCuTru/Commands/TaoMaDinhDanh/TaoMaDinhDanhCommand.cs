using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoMaDinhDanh;

public record TaoMaDinhDanhCommand(
    int QuanHeCuTruId,
    string Email) : ICommand<string>;
