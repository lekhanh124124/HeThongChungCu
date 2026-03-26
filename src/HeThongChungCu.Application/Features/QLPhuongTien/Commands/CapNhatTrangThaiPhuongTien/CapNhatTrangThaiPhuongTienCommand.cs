using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.CapNhatTrangThaiPhuongTien;

public sealed record CapNhatTrangThaiPhuongTienCommand(
    List<int> PhuongTienIds,
    int TrangThaiPhuongTienId
) : ICommand<bool>;
