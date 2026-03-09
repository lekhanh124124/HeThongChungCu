using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.ChungCu.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.ChungCu.Commands.UpdateCanHo;

public class UpdateCanHoCommandHandler : ICommandHandler<UpdateCanHoCommand, CanHoResponse>
{
    private readonly ICanHoEFRepository _canHoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCanHoCommandHandler(ICanHoEFRepository canHoRepository, IUnitOfWork unitOfWork)
    {
        _canHoRepository = canHoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CanHoResponse>> Handle(UpdateCanHoCommand request, CancellationToken cancellationToken)
    {
        var canHo = await _canHoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (canHo is null)
            return Result.Failure<CanHoResponse>(CanHoErrors.NotFoundById(request.Id));

        canHo.UpdateInfo(request.DienTich, request.Tang, request.SoPhongNgu, request.SoPhongTam, request.LoaiCanHoId);
        canHo.UpdateStatus(request.TinhTrangCanHoId);
        _canHoRepository.Update(canHo);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CanHoResponse
        {
            Id = canHo.Id,
            ToaNhaId = canHo.ToaNhaId,
            MaCanHo = canHo.MaCanHo,
            DienTich = canHo.DienTich,
            Tang = canHo.Tang,
            SoPhongNgu = canHo.SoPhongNgu,
            SoPhongTam = canHo.SoPhongTam,
            TinhTrangCanHoId = canHo.TinhTrangCanHoId
        });
    }
}
