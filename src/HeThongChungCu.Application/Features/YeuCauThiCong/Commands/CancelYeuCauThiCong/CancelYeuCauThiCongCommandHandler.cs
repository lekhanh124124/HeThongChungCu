using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;
using HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetYeuCauThiCongById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.CancelYeuCauThiCong;

public class CancelYeuCauThiCongCommandHandler : ICommandHandler<CancelYeuCauThiCongCommand, YeuCauThiCongResponse>
{
    private readonly IYeuCauThiCongCommandRepository _yctcCommandRepository;
    private readonly IYeuCauThiCongQueryRepository _yctcQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CancelYeuCauThiCongCommandHandler(
        IYeuCauThiCongCommandRepository yctcCommandRepository,
        IYeuCauThiCongQueryRepository yctcQueryRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider)
    {
        _yctcCommandRepository = yctcCommandRepository;
        _yctcQueryRepository = yctcQueryRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<YeuCauThiCongResponse>> Handle(CancelYeuCauThiCongCommand command, CancellationToken cancellationToken)
    {
        var yctc = await _yctcCommandRepository.GetByIdAsync(command.Id, cancellationToken);
        if (yctc == null)
            return YeuCauThiCongErrors.NotFound;

        var adminId = _currentUserService.UserId;
        if (adminId == null)
            return UserErrors.NotFound;

        var processedAt = _dateTimeProvider.UtcNow;

        var result = yctc.Cancel(adminId.Value, command.LyDo, processedAt);
        if (result.IsFailure)
            return result.Errors;

        _yctcCommandRepository.Update(yctc);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = await _yctcQueryRepository.GetByIdAsync(new GetYeuCauThiCongByIdSpecification(yctc.Id), cancellationToken);
        return response != null ? response : YeuCauThiCongErrors.NotFound;
    }
}
