using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;

using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.ActivateKhungGioDichVu;

public class ActivateKhungGioDichVuCommandHandler : ICommandHandler<ActivateKhungGioDichVuCommand, bool>
{
    private readonly IDichVuCommandRepository _dichVuCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateKhungGioDichVuCommandHandler(IDichVuCommandRepository dichVuCommandRepository, IUnitOfWork unitOfWork)
    {
        _dichVuCommandRepository = dichVuCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ActivateKhungGioDichVuCommand request, CancellationToken cancellationToken)
    {
        var dichVu = await _dichVuCommandRepository.GetByIdWithKhungGiosAsync(request.DichVuId, cancellationToken);
        if (dichVu == null)
            return Result.Failure<bool>(DichVuErrors.NotFoundById(request.DichVuId));

        foreach (var id in request.Ids)
        {
            var result = dichVu.ActivateKhungGio(id);
            if (result.IsFailure)
            {
                return Result.Failure<bool>(result.Errors);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }
}
