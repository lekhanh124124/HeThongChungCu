using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using MediatR;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.RevokeHopDong;

public record RevokeHopDongCommand : ICommand<bool>
{
    public int DoiTacId { get; init; }
    public List<int> Ids { get; init; } = [];
}
