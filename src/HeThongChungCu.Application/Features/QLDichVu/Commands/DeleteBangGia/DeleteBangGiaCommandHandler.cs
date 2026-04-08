using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;

using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.DeleteBangGia;

public class DeleteBangGiaCommandHandler : ICommandHandler<DeleteBangGiaCommand, bool>
{
    private readonly IDichVuCommandRepository _dichVuCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBangGiaCommandHandler(IDichVuCommandRepository dichVuCommandRepository, IUnitOfWork unitOfWork)
    {
        _dichVuCommandRepository = dichVuCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteBangGiaCommand request, CancellationToken cancellationToken)
    {
        var dichVu = await _dichVuCommandRepository.GetByIdWithBangGiasAsync(request.DichVuId, cancellationToken);
        if (dichVu == null)
            return Result.Failure<bool>(DichVuErrors.NotFoundById(request.DichVuId));

        foreach (var id in request.Ids)
        {
            dichVu.RemoveBangGia(id);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }
}
