namespace HeThongChungCu.Application.Features.QuanHeCuTru.Commands.KetThucCuTru;

public class KetThucCuTruCommandHandler : ICommandHandler<KetThucCuTruCommand, bool>
{
    private readonly ICanHoEFRepository _canHoRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public KetThucCuTruCommandHandler(
        ICanHoEFRepository canHoRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _canHoRepository = canHoRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<bool>> Handle(KetThucCuTruCommand request, CancellationToken cancellationToken)
    {
        var canHo = await _canHoRepository.GetByIdWithQuanHeForRecordAsync(request.QuanHeCuTruId, cancellationToken);
        if (canHo is null)
            return Result.Failure<bool>(QuanHeCuTruErrors.NotFoundById(request.QuanHeCuTruId));

        var quanHe = canHo.QuanHeCuTrus.FirstOrDefault(q => q.Id == request.QuanHeCuTruId);
        if (quanHe is null)
            return Result.Failure<bool>(QuanHeCuTruErrors.NotFoundById(request.QuanHeCuTruId));

        // TODO: Kiểm tra công nợ của quan hệ này, nếu là chủ hộ thì kiểm tra cả căn hộ.
        // TODO: Vô hiệu hóa các thẻ phương tiện của quan hệ này, nếu là chủ hộ thì vô hiệu hóa cả thẻ phương tiện của căn hộ.
        // TODO: Nếu là chủ hộ thì kết thúc gửi phương tiện của căn hộ.
        // TODO: Nếu là chủ hộ thì kết thúc các quan hệ cư trú khác của căn hộ.
        // TODO: Cập nhật trạng thái căn hộ nếu không còn chủ hộ nào khác.

        if (quanHe.IsKetThuc)
            return Result.Failure<bool>(QuanHeCuTruErrors.CuTruDaKetThuc);

        var now = _dateTimeProvider.Now.DateTime;
        quanHe.KetThucCuTru(now);
        _canHoRepository.Update(canHo);

        // TransactionBehavior will automatically save changes when the scope ends, so there is no need to call _unitOfWork.SaveChangesAsync() here

        return Result.Success(true);
    }
}
