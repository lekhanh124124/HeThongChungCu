using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKetQuaKhaoSat;

public class GetKetQuaKhaoSatQueryHandler : IQueryHandler<GetKetQuaKhaoSatQuery, KetQuaKhaoSatResponse>
{
    private readonly IKhaoSatQueryRepository _queryRepository;

    public GetKetQuaKhaoSatQueryHandler(IKhaoSatQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<KetQuaKhaoSatResponse>> Handle(GetKetQuaKhaoSatQuery query, CancellationToken cancellationToken)
    {
        var spec = new GetKetQuaKhaoSatSpecification(query.Id);
        var response = await _queryRepository.GetKetQuaKhaoSatAsync(spec, cancellationToken);

        return response != null
            ? Result.Success(response)
            : Result.Failure<KetQuaKhaoSatResponse>(KhaoSatErrors.NotFound);
    }
}
