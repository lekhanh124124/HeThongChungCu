using HeThongChungCu.Domain.Common;
using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.RevokeBangGia;

public record RevokeBangGiaCommand : ICommand<bool>
{
    public int DichVuId { get; init; }
    public List<int> Ids { get; init; } = [];
}
