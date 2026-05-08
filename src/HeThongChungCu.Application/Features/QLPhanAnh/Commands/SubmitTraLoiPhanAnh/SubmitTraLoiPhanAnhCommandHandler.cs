using System.Linq;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;
using HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.SubmitTraLoiPhanAnh;

public class SubmitTraLoiPhanAnhCommandHandler : ICommandHandler<SubmitTraLoiPhanAnhCommand, PhanAnhResponse>
{
    private readonly IYeuCauPhanAnhCommandRepository _phanAnhCommandRepository;
    private readonly IYeuCauPhanAnhQueryRepository _phanAnhQueryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitTraLoiPhanAnhCommandHandler(
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

    public async Task<Result<PhanAnhResponse>> Handle(SubmitTraLoiPhanAnhCommand command, CancellationToken cancellationToken)
    {
        // 1. Fetch Feedback with existing replies
        var phanAnh = await _phanAnhCommandRepository.GetByIdWithRepliesAsync(command.PhanAnhId, cancellationToken);
        if (phanAnh == null)
            return PhanAnhErrors.NotFound;

        // 2. Resolve isNhanVien flag from token claims directly for security
        var isNhanVien = _currentUserService.Roles.Any(r => 
            r == Role.Staff.Name || 
            r == Role.Manager.Name || 
            r == Role.Admin.Name);

        // 3. Domain logic (ThemPhanHoi)
        var result = phanAnh.ThemPhanHoi(command.NoiDung, isNhanVien);
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
