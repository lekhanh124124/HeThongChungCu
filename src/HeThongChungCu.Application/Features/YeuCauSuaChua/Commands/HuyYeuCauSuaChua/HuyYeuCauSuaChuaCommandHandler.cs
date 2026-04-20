using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetYeuCauSuaChuaById;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.HuyYeuCauSuaChua;

public class HuyYeuCauSuaChuaCommandHandler : ICommandHandler<HuyYeuCauSuaChuaCommand, YeuCauSuaChuaDetailResponse>
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly IYeuCauSuaChuaQueryRepository _queryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public HuyYeuCauSuaChuaCommandHandler(
        IYeuCauSuaChuaCommandRepository ycscRepository,
        IYeuCauSuaChuaQueryRepository queryRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _ycscRepository = ycscRepository;
        _queryRepository = queryRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauSuaChuaDetailResponse>> Handle(HuyYeuCauSuaChuaCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Request
        var ycsc = await _ycscRepository.GetByIdAsync(request.Id, cancellationToken);
        if (ycsc == null)
            return Result.Failure<YeuCauSuaChuaDetailResponse>(YeuCauSuaChuaErrors.NotFoundById(request.Id));

        // 2. Logic Check (Role-based state restriction)
        var isResident = _currentUserService.Roles.Contains(Role.Resident.Name);
        if (isResident)
        {
            return Result.Failure<YeuCauSuaChuaDetailResponse>(YeuCauSuaChuaErrors.HuyForbidden);
        }

        // 3. Perform Cancel
        ycsc.Huy(request.LyDoHuy);

        // 4. Persistence
        _ycscRepository.Update(ycsc);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Build Response using Query Repository
        var result = await _queryRepository.GetByIdAsync(new GetYeuCauSuaChuaByIdSpecification(ycsc.Id), cancellationToken);

        return result != null
            ? Result.Success(result)
            : Result.Failure<YeuCauSuaChuaDetailResponse>(YeuCauSuaChuaErrors.NotFoundById(ycsc.Id));
    }
}
