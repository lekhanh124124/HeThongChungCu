using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetBangGiaById;

public class GetBangGiaByIdQueryHandler : IQueryHandler<GetBangGiaByIdQuery, BangGiaResponse?>
{
    private readonly IDichVuQueryRepository _queryRepository;

    public GetBangGiaByIdQueryHandler(IDichVuQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<BangGiaResponse?>> Handle(GetBangGiaByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetBangGiaByIdSpecification(request.Id);
        return Result.Success(await _queryRepository.GetBangGiaByIdAsync(spec, cancellationToken));
    }
}
