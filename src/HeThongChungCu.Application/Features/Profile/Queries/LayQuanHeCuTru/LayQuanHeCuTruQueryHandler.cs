using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Domain.Common;
using MediatR;

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

        var result = await _repository.GetActiveByUserIdAsync(userId.Value, cancellationToken);
        return Result.Success(result);
    }
}
