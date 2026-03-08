using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Commands.KetThucCuTru;

public class KetThucCuTruCommandHandler : ICommandHandler<KetThucCuTruCommand, bool>
{
    private readonly ICanHoEFRepository _canHoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public KetThucCuTruCommandHandler(ICanHoEFRepository canHoRepository, IUnitOfWork unitOfWork)
    {
        _canHoRepository = canHoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(KetThucCuTruCommand request, CancellationToken cancellationToken)
    {
        var canHo = await _canHoRepository.GetByIdWithQuanHeForRecordAsync(request.QuanHeCuTruId, cancellationToken);
        if (canHo is null)
            return Result.Failure<bool>(QuanHeCuTruErrors.NotFoundById(request.QuanHeCuTruId));

        var quanHe = canHo.QuanHeCuTrus.FirstOrDefault(q => q.Id == request.QuanHeCuTruId);
        if (quanHe is null)
            return Result.Failure<bool>(QuanHeCuTruErrors.NotFoundById(request.QuanHeCuTruId));

        if (!quanHe.TrangThai)
            return Result.Failure<bool>(QuanHeCuTruErrors.CuTruDaKetThuc);

        quanHe.KetThucCuTru(request.NgayKetThuc);
        _canHoRepository.Update(canHo);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
