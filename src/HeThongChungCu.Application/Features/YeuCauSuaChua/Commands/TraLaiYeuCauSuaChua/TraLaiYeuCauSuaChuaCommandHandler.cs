using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetYeuCauSuaChuaById;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.TraLaiYeuCauSuaChua;

public class TraLaiYeuCauSuaChuaCommandHandler : ICommandHandler<TraLaiYeuCauSuaChuaCommand, YeuCauSuaChuaDetailResponse>
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly IYeuCauSuaChuaQueryRepository _queryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public TraLaiYeuCauSuaChuaCommandHandler(
        IYeuCauSuaChuaCommandRepository ycscRepository,
        IYeuCauSuaChuaQueryRepository queryRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _ycscRepository = ycscRepository;
        _queryRepository = queryRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauSuaChuaDetailResponse>> Handle(TraLaiYeuCauSuaChuaCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch aggregate
        var ycsc = await _ycscRepository.GetByIdAsync(request.Id, cancellationToken);
        if (ycsc == null)
            return YeuCauSuaChuaErrors.NotFoundById(request.Id);

        // 2. Resolve admin ID
        var userId = _currentUserService.UserId;
        if (userId == null)
            return UserErrors.NotFound;

        // 3. Domain logic
        var returnResult = ycsc.Return(userId.Value, request.LyDo, _dateTimeProvider.UtcNow);
        if (returnResult.IsFailure)
            return returnResult.Errors;

        // 4. Persistence
        _ycscRepository.Update(ycsc);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Build Response
        var result = await _queryRepository.GetByIdAsync(
            new GetYeuCauSuaChuaByIdSpecification(ycsc.Id),
            cancellationToken
        );

        return result != null
            ? result
            : YeuCauSuaChuaErrors.NotFoundById(ycsc.Id);
    }
}
