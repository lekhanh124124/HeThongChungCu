using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetDichVuById;

public class GetDichVuByIdQueryHandler : IQueryHandler<GetDichVuByIdQuery, DichVuDetailResponse>
{
    private readonly IDichVuQueryRepository _dichVuQueryRepository;

    public GetDichVuByIdQueryHandler(IDichVuQueryRepository dichVuQueryRepository)
    {
        _dichVuQueryRepository = dichVuQueryRepository;
    }

    public async Task<Result<DichVuDetailResponse>> Handle(GetDichVuByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetDichVuByIdSpecification(request.Id);
        var result = await _dichVuQueryRepository.GetByIdAsync(spec, cancellationToken);
        if (result == null)
            return Result.Failure<DichVuDetailResponse>(Error.NotFound("DichVu", request.Id));

        return Result.Success(result);
    }
}
