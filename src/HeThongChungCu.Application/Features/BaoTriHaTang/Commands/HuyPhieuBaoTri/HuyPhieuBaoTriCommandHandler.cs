using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.HuyPhieuBaoTri;

public class HuyPhieuBaoTriCommandHandler : ICommandHandler<HuyPhieuBaoTriCommand, bool>
{
    private readonly IPhieuBaoTriCommandRepository _phieuBaoTriRepository;
    private readonly IThietBiCommandRepository _thietBiRepository;
    private readonly IUnitOfWork _unitOfWork;

    public HuyPhieuBaoTriCommandHandler(
        IPhieuBaoTriCommandRepository phieuBaoTriRepository,
        IThietBiCommandRepository thietBiRepository,
        IUnitOfWork unitOfWork)
    {
        _phieuBaoTriRepository = phieuBaoTriRepository;
        _thietBiRepository = thietBiRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(HuyPhieuBaoTriCommand request, CancellationToken cancellationToken)
    {
        var phieu = await _phieuBaoTriRepository.GetPhieuBaoTriByIdAsync(request.Id, cancellationToken);
        if (phieu == null)
            return BaoTriHaTangErrors.PhieuBaoTriNotFoundById(request.Id);

        var thietBi = await _thietBiRepository.GetThietBiByIdAsync(phieu.ThietBiId, cancellationToken);
        if (thietBi == null)
            return BaoTriHaTangErrors.ThietBiNotFoundById(phieu.ThietBiId);

        phieu.Cancel(request.LyDo);
        
        thietBi.UpdateTrangThai(TrangThaiThietBi.HoatDongTot);
        _thietBiRepository.UpdateThietBi(thietBi);

        _phieuBaoTriRepository.UpdatePhieuBaoTri(phieu);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
