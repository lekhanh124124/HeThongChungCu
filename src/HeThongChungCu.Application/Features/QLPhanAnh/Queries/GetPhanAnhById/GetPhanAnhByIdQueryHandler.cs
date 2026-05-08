using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhById;

public class GetPhanAnhByIdQueryHandler : IQueryHandler<GetPhanAnhByIdQuery, PhanAnhDetailResponse>
{
    private readonly IYeuCauPhanAnhQueryRepository _queryRepository;

    public GetPhanAnhByIdQueryHandler(IYeuCauPhanAnhQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PhanAnhDetailResponse>> Handle(GetPhanAnhByIdQuery query, CancellationToken cancellationToken)
    {
        var spec = new GetPhanAnhByIdSpecification(query.Id);
        var response = await _queryRepository.GetByIdAsync(spec, cancellationToken);

        return response != null
            ? Result.Success(response)
            : Result.Failure<PhanAnhDetailResponse>(PhanAnhErrors.NotFound);
    }
}
