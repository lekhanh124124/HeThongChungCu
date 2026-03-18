namespace HeThongChungCu.Application.Features.PhuongTien.Commands.DeletePhuongTien;

internal sealed class DeletePhuongTienCommandHandler : ICommandHandler<DeletePhuongTienCommand, bool>
{
    private readonly IPhuongTienEFRepository _phuongTienEFRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePhuongTienCommandHandler(
        IPhuongTienEFRepository phuongTienEFRepository,
        IUnitOfWork unitOfWork)
    {
        _phuongTienEFRepository = phuongTienEFRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeletePhuongTienCommand request, CancellationToken cancellationToken)
    {
        var phuongTiens = await _phuongTienEFRepository.GetPhuongTiensByIdsAsync(request.Ids, cancellationToken);
        
        if (phuongTiens.Count == 0)
            return Result.Failure<bool>(PhuongTienErrors.NotFound);

        foreach (var phuongTien in phuongTiens)
        {
            phuongTien.Xoa();
        }

        _phuongTienEFRepository.RemoveRange(phuongTiens);

        // TransactionBehavior will automatically commit if no exception is thrown, otherwise it will rollback

        return Result.Success(true);
    }
}
