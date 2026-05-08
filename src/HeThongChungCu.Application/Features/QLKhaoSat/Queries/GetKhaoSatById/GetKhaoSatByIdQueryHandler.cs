using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

using HeThongChungCu.Application.Common.Interfaces.Services;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatById;

public class GetKhaoSatByIdQueryHandler : IQueryHandler<GetKhaoSatByIdQuery, KhaoSatDetailResponse>
{
    private readonly IKhaoSatQueryRepository _queryRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetKhaoSatByIdQueryHandler(IKhaoSatQueryRepository queryRepository, ICurrentUserService currentUserService)
    {
        _queryRepository = queryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<KhaoSatDetailResponse>> Handle(GetKhaoSatByIdQuery query, CancellationToken cancellationToken)
    {
        var spec = new GetKhaoSatByIdSpecification(query.Id, _currentUserService.UserId);
        var response = await _queryRepository.GetByIdAsync(spec, cancellationToken);

        return response != null
            ? Result.Success(response)
            : Result.Failure<KhaoSatDetailResponse>(KhaoSatErrors.NotFound);
    }
}
