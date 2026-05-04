using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.DeleteChiSoTieuThu;

public class DeleteChiSoTieuThuCommandHandler : ICommandHandler<DeleteChiSoTieuThuCommand, int>
{
    private readonly IChiSoTieuThuCommandRepository _chiSoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteChiSoTieuThuCommandHandler(IChiSoTieuThuCommandRepository chiSoRepository, IUnitOfWork unitOfWork)
    {
        _chiSoRepository = chiSoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(DeleteChiSoTieuThuCommand request, CancellationToken cancellationToken)
    {
        var chiSos = await _chiSoRepository.GetByIdsAsync(request.Ids, cancellationToken);

        var foundIds = chiSos.Select(x => x.Id).ToList();
        var missingIds = request.Ids.Except(foundIds).ToList();

        if (missingIds.Count != 0)
        {
            return ChiSoTieuThuErrors.NotFoundByIds(missingIds.ToArray());
        }

        _chiSoRepository.RemoveRange(chiSos);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return chiSos.Count;
    }
}
