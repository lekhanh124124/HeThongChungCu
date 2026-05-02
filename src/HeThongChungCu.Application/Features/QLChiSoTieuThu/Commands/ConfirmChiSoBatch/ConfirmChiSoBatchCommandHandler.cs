using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.ConfirmChiSoBatch;

public class ConfirmChiSoBatchCommandHandler : ICommandHandler<ConfirmChiSoBatchCommand, int>
{
    private readonly IChiSoTieuThuCommandRepository _chiSoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmChiSoBatchCommandHandler(IChiSoTieuThuCommandRepository chiSoRepository, IUnitOfWork unitOfWork)
    {
        _chiSoRepository = chiSoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(ConfirmChiSoBatchCommand request, CancellationToken cancellationToken)
    {
        var count = 0;
        foreach (var id in request.ChiSoIds)
        {
            var chiSo = await _chiSoRepository.GetByIdAsync(id, cancellationToken);
            if (chiSo != null)
            {
                var result = chiSo.Confirm();
                if (result.IsSuccess)
                {
                    _chiSoRepository.Update(chiSo);
                    count++;
                }
            }
        }

        if (count > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success(count);
    }
}
