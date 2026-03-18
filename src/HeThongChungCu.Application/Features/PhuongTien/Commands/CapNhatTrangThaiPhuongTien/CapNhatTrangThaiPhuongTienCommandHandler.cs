namespace HeThongChungCu.Application.Features.PhuongTien.Commands.CapNhatTrangThaiPhuongTien;

internal sealed class CapNhatTrangThaiPhuongTienCommandHandler : ICommandHandler<CapNhatTrangThaiPhuongTienCommand, bool>
{
    private readonly IPhuongTienEFRepository _phuongTienEFRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CapNhatTrangThaiPhuongTienCommandHandler(
        IPhuongTienEFRepository phuongTienEFRepository,
        IUnitOfWork unitOfWork)
    {
        _phuongTienEFRepository = phuongTienEFRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(CapNhatTrangThaiPhuongTienCommand request, CancellationToken cancellationToken)
    {
        var phuongTiens = await _phuongTienEFRepository.GetPhuongTiensByIdsAsync(request.PhuongTienIds, cancellationToken);

        if (phuongTiens.Count == 0)
        {
            return Result.Failure<bool>(PhuongTienErrors.NotFound);
        }

        var trangThai = TrangThaiPhuongTien.FromValue(request.TrangThaiPhuongTienId)!;

        foreach (var phuongTien in phuongTiens)
        {
            phuongTien.UpdateTrangThai(trangThai);
            _phuongTienEFRepository.Update(phuongTien);
        }

        // TransactionBehavior will automatically commit if no exception is thrown, otherwise it will rollback

        return Result.Success(true);
    }
}
