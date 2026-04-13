using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetYeuCauPhuongTienById;

public class GetYeuCauPhuongTienByIdQueryHandler : IQueryHandler<GetYeuCauPhuongTienByIdQuery, YeuCauPhuongTienResponse>
{
    private readonly IYeuCauPhuongTienQueryRepository _yeuCauRepository;

    public GetYeuCauPhuongTienByIdQueryHandler(IYeuCauPhuongTienQueryRepository yeuCauRepository)
    {
        _yeuCauRepository = yeuCauRepository;
    }

    public async Task<Result<YeuCauPhuongTienResponse>> Handle(GetYeuCauPhuongTienByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetYeuCauPhuongTienByIdSpecification(request.RequestId);
        var response = await _yeuCauRepository.GetByIdAsync(spec, cancellationToken);

        if (response == null)
            return Result.Failure<YeuCauPhuongTienResponse>(YeuCauPhuongTienErrors.NotFound);

        return Result.Success(response);
    }
}
