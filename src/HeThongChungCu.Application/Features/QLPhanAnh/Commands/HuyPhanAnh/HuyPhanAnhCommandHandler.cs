using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;
using HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.HuyPhanAnh;

public class HuyPhanAnhCommandHandler : ICommandHandler<HuyPhanAnhCommand, PhanAnhResponse>
{
    private readonly IYeuCauPhanAnhCommandRepository _phanAnhCommandRepository;
    private readonly IYeuCauPhanAnhQueryRepository _phanAnhQueryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public HuyPhanAnhCommandHandler(
        IYeuCauPhanAnhCommandRepository phanAnhCommandRepository,
        IYeuCauPhanAnhQueryRepository phanAnhQueryRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _phanAnhCommandRepository = phanAnhCommandRepository;
        _phanAnhQueryRepository = phanAnhQueryRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PhanAnhResponse>> Handle(HuyPhanAnhCommand request, CancellationToken cancellationToken)
    {
        // 1. Authenticate BQL/Admin user
        var adminId = _currentUserService.UserId;
        if (adminId == null)
            return Result.Failure<PhanAnhResponse>(UserErrors.NotFound);

        // 2. Fetch complaint
        var phanAnh = await _phanAnhCommandRepository.GetByIdAsync(request.PhanAnhId, cancellationToken);
        if (phanAnh == null)
            return Result.Failure<PhanAnhResponse>(PhanAnhErrors.NotFoundById(request.PhanAnhId));

        // 3. Domain Cancel transition
        var cancelResult = phanAnh.Cancel(adminId.Value, request.LyDoHuy, DateTimeOffset.UtcNow);
        if (cancelResult.IsFailure)
            return Result.Failure<PhanAnhResponse>(cancelResult.Errors);

        // 4. Persistence
        _phanAnhCommandRepository.Update(phanAnh);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Query updated detailed record
        var response = await _phanAnhQueryRepository.GetByIdAsync(new GetPhanAnhByIdSpecification(phanAnh.Id), cancellationToken);

        return response != null
            ? Result.Success<PhanAnhResponse>(response)
            : Result.Failure<PhanAnhResponse>(PhanAnhErrors.NotFound);
    }
}
