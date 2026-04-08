using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.DeleteDichVu;

public class DeleteDichVuCommandHandler : ICommandHandler<DeleteDichVuCommand, bool>
{
    private readonly IDichVuCommandRepository _dichVuCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDichVuCommandHandler(IDichVuCommandRepository dichVuCommandRepository, IUnitOfWork unitOfWork)
    {
        _dichVuCommandRepository = dichVuCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteDichVuCommand request, CancellationToken cancellationToken)
    {
        var dichVus = await _dichVuCommandRepository.GetByIdsWithAllAsync(request.Ids, cancellationToken);
        var foundIds = dichVus.Select(x => x.Id).ToList();
        var missingIds = request.Ids.Except(foundIds).ToList();

        if (missingIds.Count != 0)
        {
            return Result.Failure<bool>(DichVuErrors.NotFoundByIds(missingIds));
        }

        foreach (var dichVu in dichVus)
        {
            foreach (var bg in dichVu.BangGias)
            {
                _dichVuCommandRepository.RemoveBangGia(bg);
            }

            foreach (var kg in dichVu.KhungGios)
            {
                _dichVuCommandRepository.RemoveKhungGio(kg);
            }

            _dichVuCommandRepository.Remove(dichVu);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }
}
