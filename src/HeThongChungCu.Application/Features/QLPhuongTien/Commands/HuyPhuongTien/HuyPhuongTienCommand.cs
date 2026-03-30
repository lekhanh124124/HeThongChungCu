using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.HuyPhuongTien;

public sealed record HuyPhuongTienCommand(
    List<int> PhuongTienIds
) : ICommand<bool>;
