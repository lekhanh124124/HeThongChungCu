using HeThongChungCu.Application.Features.CuDan.DTOs;

namespace HeThongChungCu.Application.Features.CuDan.Queries.LayQuanHeCuTru;

public class LayQuanHeCuTruQueryHandler : IQueryHandler<LayQuanHeCuTruQuery, IReadOnlyList<QuanHeCuTruResponse>>
{
    private readonly IQuanHeCuTruDapperRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public LayQuanHeCuTruQueryHandler(IQuanHeCuTruDapperRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IReadOnlyList<QuanHeCuTruResponse>>> Handle(LayQuanHeCuTruQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result.Failure<IReadOnlyList<QuanHeCuTruResponse>>(UserErrors.NotFound);
        }

        var spec = new LayQuanHeCuTruSpecification(userId.Value);
        var result = await _repository.GetActiveByUserIdAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
