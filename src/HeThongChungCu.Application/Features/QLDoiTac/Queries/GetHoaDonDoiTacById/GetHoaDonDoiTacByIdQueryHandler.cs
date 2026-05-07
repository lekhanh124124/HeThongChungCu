using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDoiTac.Queries.GetHoaDonDoiTacById;

public class GetHoaDonDoiTacByIdQueryHandler : IQueryHandler<GetHoaDonDoiTacByIdQuery, HoaDonDoiTacDetailResponse>
{
    private readonly IHoaDonDoiTacQueryRepository _queryRepository;

    public GetHoaDonDoiTacByIdQueryHandler(IHoaDonDoiTacQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<HoaDonDoiTacDetailResponse>> Handle(
        GetHoaDonDoiTacByIdQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new GetHoaDonDoiTacByIdSpecification(request.Id);
        var result = await _queryRepository.GetByIdAsync(spec, cancellationToken);

        if (result == null)
        {
            return Result.Failure<HoaDonDoiTacDetailResponse>(DoiTacErrors.HoaDonNotFound);
        }

        return Result.Success(result);
    }
}
