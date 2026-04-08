using HeThongChungCu.Domain.Common;
using MediatR;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.RevokeBangGia;

public record RevokeBangGiaCommand : IRequest<Result<bool>>
{
    public int DichVuId { get; init; }
    public List<int> Ids { get; init; } = [];
}
