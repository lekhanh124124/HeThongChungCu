using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetResidentSurveyHistory;

public class GetResidentSurveyHistoryQueryHandler : IQueryHandler<GetResidentSurveyHistoryQuery, List<ResidentSurveyHistoryResponse>>
{
    private readonly IKhaoSatQueryRepository _queryRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetResidentSurveyHistoryQueryHandler(
        IKhaoSatQueryRepository queryRepository,
        ICurrentUserService currentUserService)
    {
        _queryRepository = queryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<ResidentSurveyHistoryResponse>>> Handle(GetResidentSurveyHistoryQuery query, CancellationToken cancellationToken)
    {
        var spec = new GetResidentSurveyHistorySpecification(query.CanHoId, query.KhaoSatId);
        var response = await _queryRepository.GetResidentHistoryAsync(spec, cancellationToken);
        
        return Result.Success(response);
    }
}
