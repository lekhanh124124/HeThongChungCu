using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.CanHo.Commands.UpdateCanHo;

public class UpdateCanHoCommandHandler : ICommandHandler<UpdateCanHoCommand, CanHoDetailResponse>
{
    private readonly ICanHoEFRepository _canHoRepository;
    private readonly ITangEFRepository _tangRepository;

    public UpdateCanHoCommandHandler(
        ICanHoEFRepository canHoRepository,
        ITangEFRepository tangRepository)
    {
        _canHoRepository = canHoRepository;
        _tangRepository = tangRepository;
    }

    public async Task<Result<CanHoDetailResponse>> Handle(UpdateCanHoCommand request, CancellationToken cancellationToken)
    {
        var canHo = await _canHoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (canHo is null)
            return Result.Failure<CanHoDetailResponse>(CanHoErrors.NotFoundById(request.Id));

        var tang = await _tangRepository.GetByIdAsync(request.TangId, cancellationToken);
        if (tang == null)
            return Result.Failure<CanHoDetailResponse>(CanHoErrors.NotFound);

        if (tang.LoaiTangId == LoaiTang.TangHam.Value)
            return Result.Failure<CanHoDetailResponse>(CanHoErrors.CanHoInBasement);

        var loaiCanHo = LoaiCanHo.FromValue(request.LoaiCanHoId);
        var tinhTrangCanHo = TinhTrangCanHo.FromValue(request.TinhTrangCanHoId);

        canHo.UpdateInfo(request.DienTich, request.TangId, request.SoPhongNgu, request.SoPhongTam, loaiCanHo!.Value);
        canHo.UpdateStatus(tinhTrangCanHo!.Value);

        _canHoRepository.Update(canHo);

        return Result.Success(new CanHoDetailResponse
        {
            Id = canHo.Id,
            MaCanHo = canHo.MaCanHo,
            DienTich = canHo.DienTich,
            TangId = canHo.TangId,
            TenTang = canHo.Tang?.TenTang ?? string.Empty,
            SoPhongNgu = canHo.SoPhongNgu,
            SoPhongTam = canHo.SoPhongTam,
            LoaiCanHoId = loaiCanHo.Value,
            TenLoaiCanHo = loaiCanHo.Name,
            TinhTrangCanHoId = tinhTrangCanHo.Value,
            TenTinhTrangCanHo = tinhTrangCanHo.Name
        });
    }
}
