using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetDotThanhToanById;

public class GetDotThanhToanByIdQueryHandler : IQueryHandler<GetDotThanhToanByIdQuery, DotThanhToanDetailResponse>
{
    private readonly IDotThanhToanQueryRepository _queryRepository;

    public GetDotThanhToanByIdQueryHandler(IDotThanhToanQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<DotThanhToanDetailResponse>> Handle(GetDotThanhToanByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetDotThanhToanByIdSpecification(request.Id);

        var result = await _queryRepository.GetByIdAsync(spec, cancellationToken);

        if (result == null)
        {
            return Result.Failure<DotThanhToanDetailResponse>(new Error("DotThanhToan.NotFound", "Không tìm thấy đợt thanh toán."));
        }

        return Result.Success(result);
    }
}
