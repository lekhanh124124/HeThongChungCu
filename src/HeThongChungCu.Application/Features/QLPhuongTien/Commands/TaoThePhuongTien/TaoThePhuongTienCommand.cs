using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TaoThePhuongTien;

public sealed record TaoThePhuongTienCommand(
    int PhuongTienId,
    string MaThe
) : ICommand<ThePhuongTienResponse>;
