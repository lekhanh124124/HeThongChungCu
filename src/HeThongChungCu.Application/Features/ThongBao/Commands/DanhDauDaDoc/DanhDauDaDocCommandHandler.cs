using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.ThongBao.Commands.DanhDauDaDoc;

public class DanhDauDaDocCommandHandler : ICommandHandler<DanhDauDaDocCommand, bool>
{
    private readonly IThongBaoCommandRepository _thongBaoCommandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DanhDauDaDocCommandHandler(
        IThongBaoCommandRepository thongBaoCommandRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider)
    {
        _thongBaoCommandRepository = thongBaoCommandRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<bool>> Handle(DanhDauDaDocCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Result.Failure<bool>(UserErrors.NotFound);

        var phanBo = await _thongBaoCommandRepository.GetPhanBoByIdAsync(request.PhanBoThongBaoId, userId.Value, cancellationToken);

        if (phanBo == null)
            return Result.Failure<bool>(new Error("ThongBao.NotFound", "Không tìm thấy thông báo hoặc bạn không có quyền truy cập."));

        phanBo.MarkAsRead(_dateTimeProvider.Now);
        _thongBaoCommandRepository.UpdatePhanBo(phanBo);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
