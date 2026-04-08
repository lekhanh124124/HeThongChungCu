using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;

namespace HeThongChungCu.Application.Features.QLDoiTac.Queries.GetDoiTacById;

public class GetDoiTacByIdQueryHandler : IQueryHandler<GetDoiTacByIdQuery, DoiTacDetailResponse>
{
    private readonly IDoiTacQueryRepository _doiTacQueryRepository;

    public GetDoiTacByIdQueryHandler(IDoiTacQueryRepository doiTacQueryRepository)
    {
        _doiTacQueryRepository = doiTacQueryRepository;
    }

    public async Task<Result<DoiTacDetailResponse>> Handle(GetDoiTacByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetDoiTacByIdSpecification(request.Id);
        var result = await _doiTacQueryRepository.GetByIdAsync(spec, cancellationToken);
        
        if (result == null)
            return Result.Failure<DoiTacDetailResponse>(Error.NotFound("DoiTac.NotFound", "Không tìm thấy đơn vị cung cấp."));

        return Result.Success(result);
    }
}
