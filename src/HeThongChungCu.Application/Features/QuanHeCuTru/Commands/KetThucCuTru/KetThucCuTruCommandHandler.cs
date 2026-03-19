namespace HeThongChungCu.Application.Features.QuanHeCuTru.Commands.KetThucCuTru;

public class KetThucCuTruCommandHandler : ICommandHandler<KetThucCuTruCommand, bool>
{
    private readonly IQuanHeCuTruEFRepository _quanHeCuTruRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public KetThucCuTruCommandHandler(
        IQuanHeCuTruEFRepository quanHeCuTruRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _quanHeCuTruRepository = quanHeCuTruRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<bool>> Handle(KetThucCuTruCommand request, CancellationToken cancellationToken)
    {
        var quanHe = await _quanHeCuTruRepository.GetCuTruByIdAsync(request.QuanHeCuTruId, cancellationToken);
        if (quanHe is null)
            return Result.Failure<bool>(QuanHeCuTruErrors.NotFoundById(request.QuanHeCuTruId));

        // TODO: Kiểm tra công nợ của quan hệ này, nếu là chủ hộ thì kiểm tra cả căn hộ.
        // TODO: Vô hiệu hóa các thẻ phương tiện của quan hệ này, nếu là chủ hộ thì vô hiệu hóa cả thẻ phương tiện của căn hộ.
        // TODO: Nếu là chủ hộ thì kết thúc gửi phương tiện của căn hộ.
        // TODO: Nếu là chủ hộ thì kết thúc các quan hệ cư trú khác của căn hộ.
        // TODO: Cập nhật trạng thái căn hộ nếu không còn chủ hộ nào khác.

        var now = _dateTimeProvider.Now.DateTime;

        quanHe.KetThucCuTru(now);

        _quanHeCuTruRepository.Update(quanHe);

        // TransactionBehavior will automatically save changes when the scope ends, so there is no need to call _unitOfWork.SaveChangesAsync() here

        return Result.Success(true);
    }
}
