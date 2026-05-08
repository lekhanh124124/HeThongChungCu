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

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.XacNhanHoanThanhPhanAnh;

public class XacNhanHoanThanhPhanAnhCommandHandler : ICommandHandler<XacNhanHoanThanhPhanAnhCommand, PhanAnhResponse>
{
    private readonly IYeuCauPhanAnhCommandRepository _phanAnhCommandRepository;
    private readonly IYeuCauPhanAnhQueryRepository _phanAnhQueryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public XacNhanHoanThanhPhanAnhCommandHandler(
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

    public async Task<Result<PhanAnhResponse>> Handle(XacNhanHoanThanhPhanAnhCommand command, CancellationToken cancellationToken)
    {
        // 1. Fetch Feedback
        var phanAnh = await _phanAnhCommandRepository.GetByIdAsync(command.PhanAnhId, cancellationToken);
        if (phanAnh == null)
            return PhanAnhErrors.NotFound;

        // 2. Fetch Current Employee/Admin ID
        var adminId = _currentUserService.UserId;
        if (adminId == null)
            return Result.Failure<PhanAnhResponse>(UserErrors.NotFound);

        // 3. Domain logic (XacNhanHoanThanh)
        var result = phanAnh.XacNhanHoanThanh(adminId.Value, command.KetQua, DateTimeOffset.UtcNow);
        if (result.IsFailure)
            return result.Errors;

        // 4. Save
        _phanAnhCommandRepository.Update(phanAnh);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Query detailed response
        var response = await _phanAnhQueryRepository.GetByIdAsync(new GetPhanAnhByIdSpecification(phanAnh.Id), cancellationToken);

        return response != null
            ? Result.Success<PhanAnhResponse>(response)
            : Result.Failure<PhanAnhResponse>(PhanAnhErrors.NotFound);
    }
}
