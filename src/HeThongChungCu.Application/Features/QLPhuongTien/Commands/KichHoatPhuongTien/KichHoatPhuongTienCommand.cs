using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.KichHoatPhuongTien;

public sealed record KichHoatPhuongTienCommand(
    List<int> PhuongTienIds
) : ICommand<bool>;
