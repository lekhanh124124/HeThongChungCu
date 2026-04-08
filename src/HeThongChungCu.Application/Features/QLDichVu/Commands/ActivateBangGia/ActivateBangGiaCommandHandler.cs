using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;

using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.ActivateBangGia;

public class ActivateBangGiaCommandHandler : ICommandHandler<ActivateBangGiaCommand, bool>
{
    private readonly IDichVuCommandRepository _dichVuCommandRepository;
    private readonly IDichVuQueryRepository _dichVuQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateBangGiaCommandHandler(
        IDichVuCommandRepository dichVuCommandRepository,
        IDichVuQueryRepository dichVuQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _dichVuCommandRepository = dichVuCommandRepository;
        _dichVuQueryRepository = dichVuQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ActivateBangGiaCommand request, CancellationToken cancellationToken)
    {
        var dichVu = await _dichVuCommandRepository.GetByIdWithBangGiasAsync(request.DichVuId, cancellationToken);
        if (dichVu == null)
            return Result.Failure<bool>(DichVuErrors.NotFoundById(request.DichVuId));

        foreach (var id in request.Ids)
        {
            dichVu.ActivateBangGia(id);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
