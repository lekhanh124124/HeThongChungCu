using HeThongChungCu.Domain.Common;
using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.RevokeKhungGioDichVu;

public record RevokeKhungGioDichVuCommand : ICommand<bool>
{
    public int DichVuId { get; init; }
    public List<int> Ids { get; init; } = [];
}
