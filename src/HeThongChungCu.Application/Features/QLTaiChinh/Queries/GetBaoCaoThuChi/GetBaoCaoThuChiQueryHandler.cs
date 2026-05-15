using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTaiChinh.DTOs;
using HeThongChungCu.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoThuChi;

public class GetBaoCaoThuChiQueryHandler : IQueryHandler<GetBaoCaoThuChiQuery, BaoCaoThuChiResponse>
{
    private readonly IQuyThuChiQueryRepository _queryRepository;

    public GetBaoCaoThuChiQueryHandler(IQuyThuChiQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<BaoCaoThuChiResponse>> Handle(GetBaoCaoThuChiQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetBaoCaoThuChiSpecification(request.TuNgay, request.DenNgay);
        var result = await _queryRepository.GetBaoCaoThuChiAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
