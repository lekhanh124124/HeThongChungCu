using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.CanHo.Commands.UpdateCanHo;

public class UpdateCanHoCommandHandler : ICommandHandler<UpdateCanHoCommand, CanHoDetailResponse>
{
    private readonly ICanHoEFRepository _canHoRepository;

    public UpdateCanHoCommandHandler(
        ICanHoEFRepository canHoRepository)
    {
        _canHoRepository = canHoRepository;
    }

    public async Task<Result<CanHoDetailResponse>> Handle(UpdateCanHoCommand request, CancellationToken cancellationToken)
    {
        var canHo = await _canHoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (canHo is null)
            return Result.Failure<CanHoDetailResponse>(CanHoErrors.NotFoundById(request.Id));

        var loaiCanHo = LoaiCanHo.FromValue(request.LoaiCanHoId);
        var tinhTrangCanHo = TinhTrangCanHo.FromValue(request.TinhTrangCanHoId);

        canHo.UpdateInfo(request.DienTich, request.Tang, request.SoPhongNgu, request.SoPhongTam, loaiCanHo!.Value);
        canHo.UpdateStatus(tinhTrangCanHo!.Value);

        _canHoRepository.Update(canHo);

        return Result.Success(new CanHoDetailResponse
        {
            Id = canHo.Id,
            ToaNhaId = canHo.ToaNhaId,
            MaCanHo = canHo.MaCanHo,
            DienTich = canHo.DienTich,
            Tang = canHo.Tang,
            SoPhongNgu = canHo.SoPhongNgu,
            SoPhongTam = canHo.SoPhongTam,
            LoaiCanHoId = loaiCanHo.Value,
            TenLoaiCanHo = loaiCanHo.Name,
            TinhTrangCanHoId = tinhTrangCanHo.Value,
            TenTinhTrangCanHo = tinhTrangCanHo.Name
        });
    }
}
