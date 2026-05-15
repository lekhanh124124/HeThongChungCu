using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTaiChinh.DTOs;
using HeThongChungCu.Domain.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoCongNoToaNha;

public class GetBaoCaoCongNoToaNhaQueryHandler : IQueryHandler<GetBaoCaoCongNoToaNhaQuery, List<BaoCaoCongNoToaNhaResponse>>
{
    private readonly IQuyThuChiQueryRepository _queryRepository;

    public GetBaoCaoCongNoToaNhaQueryHandler(IQuyThuChiQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<List<BaoCaoCongNoToaNhaResponse>>> Handle(GetBaoCaoCongNoToaNhaQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetBaoCaoCongNoToaNhaSpecification(request.Thang, request.Nam);
        var result = await _queryRepository.GetBaoCaoCongNoToaNhaAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
