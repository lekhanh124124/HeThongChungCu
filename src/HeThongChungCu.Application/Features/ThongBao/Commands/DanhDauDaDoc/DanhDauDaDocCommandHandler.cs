using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;
using MediatR;

namespace HeThongChungCu.Application.Features.ThongBao.Commands.DanhDauDaDoc;

public class DanhDauDaDocCommandHandler : IRequestHandler<DanhDauDaDocCommand, Result<bool>>
{
    private readonly IThongBaoEFRepository _thongBaoEFRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DanhDauDaDocCommandHandler(
        IThongBaoEFRepository thongBaoEFRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider)
    {
        _thongBaoEFRepository = thongBaoEFRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<bool>> Handle(DanhDauDaDocCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Result.Failure<bool>(UserErrors.NotFound);

        var phanBo = await _thongBaoEFRepository.GetPhanBoByIdAsync(request.PhanBoThongBaoId, userId.Value, cancellationToken);

        if (phanBo == null)
            return Result.Failure<bool>(new Error("ThongBao.NotFound", "Không tìm thấy thông báo hoặc bạn không có quyền truy cập."));

        phanBo.MarkAsRead(_dateTimeProvider.Now);
        _thongBaoEFRepository.UpdatePhanBo(phanBo);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
