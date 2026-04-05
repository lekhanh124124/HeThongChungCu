using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TaoThePhuongTien;

internal sealed class TaoThePhuongTienCommandHandler : ICommandHandler<TaoThePhuongTienCommand, ThePhuongTienResponse>
{
    private readonly IPhuongTienCommandRepository _phuongTienCommandRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public TaoThePhuongTienCommandHandler(
        IPhuongTienCommandRepository phuongTienCommandRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _phuongTienCommandRepository = phuongTienCommandRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ThePhuongTienResponse>> Handle(TaoThePhuongTienCommand request, CancellationToken cancellationToken)
    {
        var phuongTien = await _phuongTienCommandRepository.GetPhuongTienByIdAsync(request.PhuongTienId, cancellationToken);
        if (phuongTien == null)
            return Result.Failure<ThePhuongTienResponse>(PhuongTienErrors.NotFound);

        var maTheExists = phuongTien.ThePhuongTiens.Any(t => t.MaThe == request.MaThe);
        if (maTheExists)
            return Result.Failure<ThePhuongTienResponse>(PhuongTienErrors.MaTheExists);

        var now = _dateTimeProvider.Now.DateTime;
        var thePhuongTien = phuongTien.AddThe(request.MaThe, now);

        _phuongTienCommandRepository.Update(phuongTien);

        // TransactionBehavior will automatically commit if no exception is thrown, otherwise it will rollback

        return Result.Success(new ThePhuongTienResponse
        {
            Id = thePhuongTien.Id,
            PhuongTienId = thePhuongTien.PhuongTienId,
            MaThe = thePhuongTien.MaThe,
            NgayBatDau = thePhuongTien.ThoiGian.NgayBatDau,
            NgayKetThuc = thePhuongTien.ThoiGian.NgayKetThuc,
            TrangThaiThePhuongTienId = thePhuongTien.TrangThaiId.Value,
            TenTrangThaiThePhuongTien = thePhuongTien.TrangThaiId.Name,
        });
    }
}
