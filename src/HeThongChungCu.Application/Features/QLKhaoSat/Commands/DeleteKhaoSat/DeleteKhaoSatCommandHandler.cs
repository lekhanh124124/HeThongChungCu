using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Commands.DeleteKhaoSat;

public class DeleteKhaoSatCommandHandler : ICommandHandler<DeleteKhaoSatCommand, bool>
{
    private readonly IKhaoSatCommandRepository _khaoSatCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteKhaoSatCommandHandler(
        IKhaoSatCommandRepository khaoSatCommandRepository,
        IUnitOfWork unitOfWork)
    {
        _khaoSatCommandRepository = khaoSatCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteKhaoSatCommand command, CancellationToken cancellationToken)
    {
        // 1. Fetch survey
        var khaoSat = await _khaoSatCommandRepository.GetByIdAsync(command.Id, cancellationToken);
        if (khaoSat == null)
            return Result.Failure<bool>(KhaoSatErrors.NotFoundById(command.Id));

        // 2. Validate that it's in Draft state
        if (khaoSat.TrangThaiId != TrangThaiKhaoSat.MoiTao)
            return Result.Failure<bool>(KhaoSatErrors.NotDraftStatus);

        // 3. Delete (soft deletion is automatically handled by SaveChangesInterceptor)
        _khaoSatCommandRepository.Delete(khaoSat);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
