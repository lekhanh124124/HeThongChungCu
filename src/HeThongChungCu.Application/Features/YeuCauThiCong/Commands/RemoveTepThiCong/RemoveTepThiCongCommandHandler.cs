using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;
using HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetYeuCauThiCongById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.RemoveTepThiCong;

public class RemoveTepThiCongCommandHandler : ICommandHandler<RemoveTepThiCongCommand, YeuCauThiCongResponse>
{
    private readonly IYeuCauThiCongCommandRepository _yctcCommandRepository;
    private readonly IYeuCauThiCongQueryRepository _yctcQueryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveTepThiCongCommandHandler(
        IYeuCauThiCongCommandRepository yctcCommandRepository,
        IYeuCauThiCongQueryRepository yctcQueryRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _yctcCommandRepository = yctcCommandRepository;
        _yctcQueryRepository = yctcQueryRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauThiCongResponse>> Handle(RemoveTepThiCongCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            return UserErrors.NotFound;

        var yctc = await _yctcCommandRepository.GetByIdWithFilesAsync(command.Id, cancellationToken);
        if (yctc == null)
            return YeuCauThiCongErrors.NotFound;

        var file = yctc.TepYeuCauThiCongs.FirstOrDefault(x => x.Id == command.TepId);
        if (file != null && file.CreatedBy != userId)
            return YeuCauThiCongErrors.Forbidden;

        var result = yctc.RemoveTep(command.TepId);
        if (result.IsFailure)
            return result.Errors;

        _yctcCommandRepository.Update(yctc);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = await _yctcQueryRepository.GetByIdAsync(new GetYeuCauThiCongByIdSpecification(yctc.Id), cancellationToken);
        return response != null ? response : YeuCauThiCongErrors.NotFound;
    }
}
