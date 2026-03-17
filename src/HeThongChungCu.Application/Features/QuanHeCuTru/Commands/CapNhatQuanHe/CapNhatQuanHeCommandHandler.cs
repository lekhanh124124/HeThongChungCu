namespace HeThongChungCu.Application.Features.QuanHeCuTru.Commands.CapNhatQuanHe;

public class CapNhatQuanHeCommandHandler : ICommandHandler<CapNhatQuanHeCommand, bool>
{
    private readonly IQuanHeCuTruEFRepository _quanHeCuTruRepository;

    public CapNhatQuanHeCommandHandler(IQuanHeCuTruEFRepository quanHeCuTruRepository)
    {
        _quanHeCuTruRepository = quanHeCuTruRepository;
    }

    public async Task<Result<bool>> Handle(CapNhatQuanHeCommand request, CancellationToken cancellationToken)
    {
        var quanHe = await _quanHeCuTruRepository.GetByIdAsync(request.QuanHeCuTruId, cancellationToken);
        if (quanHe is null)
            return Result.Failure<bool>(QuanHeCuTruErrors.NotFoundById(request.QuanHeCuTruId));

        var loaiQuanHe = LoaiQuanHeCuTru.FromValue(request.LoaiQuanHeCuTruId);
        quanHe.ThayDoiLoaiQuanHe(loaiQuanHe!);
        _quanHeCuTruRepository.Update(quanHe);

        // TransactionBehavior will automatically save changes when the scope ends, so there is no need to call _unitOfWork.SaveChangesAsync() here

        return Result.Success(true);
    }
}
