using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.DeleteHangMucBaoTri;

public class DeleteHangMucBaoTriCommandHandler : ICommandHandler<DeleteHangMucBaoTriCommand, bool>
{
    private readonly IThietBiCommandRepository _thietBiRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHangMucBaoTriCommandHandler(
        IThietBiCommandRepository thietBiRepository,
        IUnitOfWork unitOfWork)
    {
        _thietBiRepository = thietBiRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteHangMucBaoTriCommand request, CancellationToken cancellationToken)
    {
        var hangMuc = await _thietBiRepository.GetHangMucByIdAsync(request.Id, cancellationToken);
        if (hangMuc is null || hangMuc.IsDeleted)
            return Result.Failure<bool>(BaoTriHaTangErrors.HangMucNotFoundById(request.Id));

        hangMuc.MarkAsDeleted(DateTimeOffset.UtcNow);
        _thietBiRepository.UpdateHangMuc(hangMuc);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
