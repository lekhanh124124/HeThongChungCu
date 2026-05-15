using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatParticipants;

public class GetKhaoSatParticipantsQueryHandler : IQueryHandler<GetKhaoSatParticipantsQuery, PagedResult<KhaoSatParticipantResponse>>
{
    private readonly IKhaoSatQueryRepository _queryRepository;

    public GetKhaoSatParticipantsQueryHandler(IKhaoSatQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<KhaoSatParticipantResponse>>> Handle(GetKhaoSatParticipantsQuery query, CancellationToken cancellationToken)
    {
        var spec = new GetKhaoSatParticipantsSpecification(query.KhaoSatId, query.PageNumber, query.PageSize);
        var response = await _queryRepository.GetParticipantsAsync(spec, cancellationToken);
        return Result.Success(response);
    }
}
