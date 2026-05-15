using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTaiChinh.DTOs;
using HeThongChungCu.Domain.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoCongNoCanHo;

public class GetBaoCaoCongNoCanHoQueryHandler : IQueryHandler<GetBaoCaoCongNoCanHoQuery, List<BaoCaoCongNoCanHoResponse>>
{
    private readonly IQuyThuChiQueryRepository _queryRepository;

    public GetBaoCaoCongNoCanHoQueryHandler(IQuyThuChiQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<List<BaoCaoCongNoCanHoResponse>>> Handle(GetBaoCaoCongNoCanHoQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetBaoCaoCongNoCanHoSpecification(request.ToaNhaId, request.Thang, request.Nam);
        var result = await _queryRepository.GetBaoCaoCongNoCanHoAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
