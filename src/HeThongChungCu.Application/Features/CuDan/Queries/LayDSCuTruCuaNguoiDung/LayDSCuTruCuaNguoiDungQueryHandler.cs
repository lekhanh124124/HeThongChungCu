using HeThongChungCu.Application.Features.CuDan.DTOs;

namespace HeThongChungCu.Application.Features.CuDan.Queries.LayDSCuTruCuaNguoiDung;

public class LayDSCuTruCuaNguoiDungQueryHandler : IQueryHandler<LayDSCuTruCuaNguoiDungQuery, IReadOnlyList<QuanHeCuTruResponse>>
{
    private readonly IQuanHeCuTruQueryRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public LayDSCuTruCuaNguoiDungQueryHandler(IQuanHeCuTruQueryRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IReadOnlyList<QuanHeCuTruResponse>>> Handle(LayDSCuTruCuaNguoiDungQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result.Failure<IReadOnlyList<QuanHeCuTruResponse>>(UserErrors.NotFound);
        }

        var spec = new LayDSCuTruCuaNguoiDungSpecification(userId.Value);
        var result = await _repository.LayDSCuTruByUserId(spec, cancellationToken);
        return Result.Success(result);
    }
}
