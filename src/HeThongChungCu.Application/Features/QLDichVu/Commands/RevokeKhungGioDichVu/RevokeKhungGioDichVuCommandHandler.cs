using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Application.Common.Messaging;

using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.RevokeKhungGioDichVu;

public class RevokeKhungGioDichVuCommandHandler : ICommandHandler<RevokeKhungGioDichVuCommand, bool>
{
    private readonly IDichVuCommandRepository _commandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeKhungGioDichVuCommandHandler(IDichVuCommandRepository commandRepository, IUnitOfWork unitOfWork)
    {
        _commandRepository = commandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(RevokeKhungGioDichVuCommand request, CancellationToken cancellationToken)
    {
        var dichVu = await _commandRepository.GetByIdWithKhungGiosAsync(request.DichVuId, cancellationToken);
        if (dichVu == null)
            return Result.Failure<bool>(DichVuErrors.NotFoundById(request.DichVuId));

        foreach (var id in request.Ids)
        {
            dichVu.DeactivateKhungGio(id);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }
}
