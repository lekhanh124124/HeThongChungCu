using HeThongChungCu.Application.Features.Profile.DTOs;

namespace HeThongChungCu.Application.Features.Profile.Queries.LayQuanHeCuTru;

public class LayQuanHeCuTruQueryHandler : IQueryHandler<LayQuanHeCuTruQuery, IReadOnlyList<LayQuanHeCuTruResponse>>
{
    private readonly IQuanHeCuTruDapperRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public LayQuanHeCuTruQueryHandler(IQuanHeCuTruDapperRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IReadOnlyList<LayQuanHeCuTruResponse>>> Handle(LayQuanHeCuTruQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result.Failure<IReadOnlyList<LayQuanHeCuTruResponse>>(new Error("User.NotFound", "User not found in context."));
        }

        var spec = new LayQuanHeCuTruSpecification(userId.Value);
        var result = await _repository.GetActiveByUserIdAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
