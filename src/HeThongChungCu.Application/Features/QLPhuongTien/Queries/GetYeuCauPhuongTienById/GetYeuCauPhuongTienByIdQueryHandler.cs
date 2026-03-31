using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetYeuCauPhuongTienById;

public class GetYeuCauPhuongTienByIdQueryHandler : IQueryHandler<GetYeuCauPhuongTienByIdQuery, YeuCauPhuongTienResponse>
{
    private readonly IYeuCauPhuongTienDapperRepository _yeuCauRepository;

    public GetYeuCauPhuongTienByIdQueryHandler(IYeuCauPhuongTienDapperRepository yeuCauRepository)
    {
        _yeuCauRepository = yeuCauRepository;
    }

    public async Task<Result<YeuCauPhuongTienResponse>> Handle(GetYeuCauPhuongTienByIdQuery request, CancellationToken cancellationToken)
    {
        var response = await _yeuCauRepository.GetByIdAsync(request.RequestId, cancellationToken);

        if (response == null)
            return Result.Failure<YeuCauPhuongTienResponse>(YeuCauPhuongTienErrors.NotFound);

        return Result.Success(response);
    }
}
