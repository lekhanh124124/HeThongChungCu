using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.KhoaPhuongTien;

public sealed record KhoaPhuongTienCommand(
    List<int> PhuongTienIds
) : ICommand<bool>;
