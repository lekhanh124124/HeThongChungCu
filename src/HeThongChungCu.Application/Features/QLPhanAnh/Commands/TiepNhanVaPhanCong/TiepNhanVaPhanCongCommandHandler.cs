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

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.TiepNhanVaPhanCong;

public class TiepNhanVaPhanCongCommandHandler : ICommandHandler<TiepNhanVaPhanCongCommand, PhanAnhResponse>
{
    private readonly IYeuCauPhanAnhCommandRepository _phanAnhCommandRepository;
    private readonly IYeuCauPhanAnhQueryRepository _phanAnhQueryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public TiepNhanVaPhanCongCommandHandler(
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

    public async Task<Result<PhanAnhResponse>> Handle(TiepNhanVaPhanCongCommand command, CancellationToken cancellationToken)
    {
        // 1. Fetch Feedback
        var phanAnh = await _phanAnhCommandRepository.GetByIdAsync(command.PhanAnhId, cancellationToken);
        if (phanAnh == null)
            return PhanAnhErrors.NotFound;

        // 2. Fetch Current Admin/Staff ID
        var adminId = _currentUserService.UserId;
        if (adminId == null)
            return Result.Failure<PhanAnhResponse>(UserErrors.NotFound);

        // 3. Domain Logic (TiepNhanVaPhanCong)
        var result = phanAnh.TiepNhanVaPhanCong(adminId.Value, DateTimeOffset.UtcNow);
        if (result.IsFailure)
            return result.Errors;

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
