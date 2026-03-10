using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Commands.CapNhatQuanHe;

public class CapNhatQuanHeCommandHandler : ICommandHandler<CapNhatQuanHeCommand, bool>
{
    private readonly ICanHoEFRepository _canHoRepository;

    public CapNhatQuanHeCommandHandler(ICanHoEFRepository canHoRepository)
    {
        _canHoRepository = canHoRepository;
    }

    public async Task<Result<bool>> Handle(CapNhatQuanHeCommand request, CancellationToken cancellationToken)
    {
        var canHo = await _canHoRepository.GetByIdWithQuanHeForRecordAsync(request.QuanHeCuTruId, cancellationToken);
        if (canHo is null)
            return Result.Failure<bool>(QuanHeCuTruErrors.NotFoundById(request.QuanHeCuTruId));

        var quanHe = canHo.QuanHeCuTrus.FirstOrDefault(q => q.Id == request.QuanHeCuTruId);
        if (quanHe is null)
            return Result.Failure<bool>(QuanHeCuTruErrors.NotFoundById(request.QuanHeCuTruId));

        if (quanHe.IsKetThuc)
            return Result.Failure<bool>(QuanHeCuTruErrors.CuTruDaKetThuc);

        quanHe.ThayDoiLoaiQuanHe(request.LoaiQuanHeCuTruId);
        _canHoRepository.Update(canHo);

        // TransactionBehavior will automatically save changes when the scope ends, so there is no need to call _unitOfWork.SaveChangesAsync() here

        return Result.Success(true);
    }
}
