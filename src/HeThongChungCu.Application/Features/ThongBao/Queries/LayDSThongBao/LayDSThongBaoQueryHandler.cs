using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Models;
using MediatR;

namespace HeThongChungCu.Application.Features.ThongBao.Queries.LayDSThongBao;

public class LayDSThongBaoQueryHandler : IRequestHandler<LayDSThongBaoQuery, Result<PagedResult<ThongBaoResponse>>>
{
    private readonly IThongBaoQueryRepository _thongBaoRepository;
    private readonly ICurrentUserService _currentUserService;

    public LayDSThongBaoQueryHandler(IThongBaoQueryRepository thongBaoRepository, ICurrentUserService currentUserService)
    {
        _thongBaoRepository = thongBaoRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PagedResult<ThongBaoResponse>>> Handle(LayDSThongBaoQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Result.Failure<PagedResult<ThongBaoResponse>>(Domain.Errors.UserErrors.NotFound);

        var result = await _thongBaoRepository.GetDSThongBaoAsync(
            userId.Value,
            request.PageNumber ?? 1,
            request.PageSize ?? 10,
            request.OnlyUnread,
            cancellationToken);

        return result;
    }
}
