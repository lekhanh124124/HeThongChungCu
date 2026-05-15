using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTaiChinh.DTOs;
using HeThongChungCu.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetQuyThuChiById;

public class GetQuyThuChiByIdQueryHandler : IQueryHandler<GetQuyThuChiByIdQuery, QuyThuChiResponse>
{
    private readonly IQuyThuChiQueryRepository _queryRepository;

    public GetQuyThuChiByIdQueryHandler(IQuyThuChiQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<QuyThuChiResponse>> Handle(GetQuyThuChiByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetQuyThuChiByIdSpecification(request.Id);
        var result = await _queryRepository.GetByIdAsync(spec, cancellationToken);
        
        if (result == null)
        {
            return Result.Failure<QuyThuChiResponse>(new Error("QuyThuChi.NotFound", $"Không tìm thấy thông tin giao dịch với ID: {request.Id}"));
        }
        
        return Result.Success(result);
    }
}
