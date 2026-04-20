using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Application.Common.Messaging;

using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.RevokeBangGia;

public class RevokeBangGiaCommandHandler : ICommandHandler<RevokeBangGiaCommand, bool>
{
    private readonly IDichVuCommandRepository _commandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeBangGiaCommandHandler(IDichVuCommandRepository commandRepository, IUnitOfWork unitOfWork)
    {
        _commandRepository = commandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(RevokeBangGiaCommand request, CancellationToken cancellationToken)
    {
        var dichVu = await _commandRepository.GetByIdWithBangGiasAsync(request.DichVuId, cancellationToken);
        if (dichVu == null)
            return Result.Failure<bool>(DichVuErrors.NotFoundById(request.DichVuId));

        foreach (var id in request.Ids)
        {
            dichVu.DeactivateBangGia(id);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }
}
