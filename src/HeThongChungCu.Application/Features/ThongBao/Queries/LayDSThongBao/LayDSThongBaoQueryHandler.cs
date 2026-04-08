using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.ThongBao.DTOs;
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

        var spec = new LayDSThongBaoSpecification(
            userId.Value,
            request.OnlyUnread,
            request.Keyword,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize);

        var result = await _thongBaoRepository.GetDSThongBaoAsync(spec, cancellationToken);

        return result;
    }
}
