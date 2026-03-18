using HeThongChungCu.Application.Features.PhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.PhuongTien.Commands.TaoThePhuongTien;

public sealed record TaoThePhuongTienCommand(
    int PhuongTienId,
    string MaThe
) : ICommand<ThePhuongTienResponse>;
