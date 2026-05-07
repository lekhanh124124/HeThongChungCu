namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.DeleteThietBi;

public class DeleteThietBiCommandHandler : ICommandHandler<DeleteThietBiCommand, bool>
{
    private readonly IThietBiCommandRepository _thietBiRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteThietBiCommandHandler(
        IThietBiCommandRepository thietBiRepository,
        IUnitOfWork unitOfWork)
    {
        _thietBiRepository = thietBiRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteThietBiCommand request, CancellationToken cancellationToken)
    {
        var thietBi = await _thietBiRepository.GetThietBiByIdAsync(request.Id, cancellationToken);
        if (thietBi == null)
            return BaoTriHaTangErrors.ThietBiNotFoundById(request.Id);

        thietBi.MarkAsDeleted(DateTimeOffset.UtcNow);
        _thietBiRepository.UpdateThietBi(thietBi);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
