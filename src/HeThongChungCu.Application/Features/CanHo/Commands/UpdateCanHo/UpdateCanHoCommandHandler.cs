using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Application.Features.Tang.DTOs;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.CanHo.Commands.UpdateCanHo;

public class UpdateCanHoCommandHandler : ICommandHandler<UpdateCanHoCommand, CanHoDetailResponse>
{
    private readonly ICanHoEFRepository _canHoRepository;
    private readonly IToaNhaEFRepository _toaNhaRepository;
    private readonly IQuanHeCuTruEFRepository _quanHeCuTruRepository;

    public UpdateCanHoCommandHandler(
        ICanHoEFRepository canHoRepository,
        IToaNhaEFRepository toaNhaRepository,
        IQuanHeCuTruEFRepository quanHeCuTruRepository)
    {
        _canHoRepository = canHoRepository;
        _toaNhaRepository = toaNhaRepository;
        _quanHeCuTruRepository = quanHeCuTruRepository;
    }

    public async Task<Result<CanHoDetailResponse>> Handle(UpdateCanHoCommand request, CancellationToken cancellationToken)
    {
        var canHo = await _canHoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (canHo is null)
            return Result.Failure<CanHoDetailResponse>(CanHoErrors.NotFoundById(request.Id));

        var loaiCanHo = LoaiCanHo.FromValue(request.LoaiCanHoId);
        var tinhTrangCanHo = TrangThaiCanHo.FromValue(request.TinhTrangCanHoId);

        // Nếu mã thay đổi, kiểm tra trùng mã
        if (request.MaCanHo != canHo.MaCanHo)
        {
            var maExists = await _canHoRepository.MaCanHoExistsAsync(request.MaCanHo, cancellationToken);
            if (maExists)
                return Result.Failure<CanHoDetailResponse>(CanHoErrors.MaCanHoAlreadyExists);
        }

        var relations = await _quanHeCuTruRepository.GetByCanHoIdAsync(canHo.Id, cancellationToken);
        bool hasActiveResidents = relations.Any(r => r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru);

        canHo.UpdateInfo(
            request.TenCanHo, 
            request.MaCanHo,
            request.DienTich, 
            request.SoPhongNgu, 
            request.SoPhongTam, 
            loaiCanHo!,
            hasActiveResidents);

        canHo.UpdateStatus(tinhTrangCanHo!);

        _canHoRepository.Update(canHo);

        var toaNha = await _toaNhaRepository.GetToaNhaByTangIdAsync(canHo.TangId, cancellationToken);
        var tang = toaNha!.Tangs.First(t => t.Id == canHo.TangId);

        return Result.Success(new CanHoDetailResponse
        {
            Id = canHo.Id,
            MaCanHo = canHo.MaCanHo,
            TenCanHo = canHo.TenCanHo,
            DienTich = canHo.DienTich,
            TangId = canHo.TangId,
            TenTang = tang.TenTang,
            SoPhongNgu = canHo.SoPhongNgu,
            SoPhongTam = canHo.SoPhongTam,
            LoaiCanHoId = canHo.LoaiCanHoId.Value,
            TenLoaiCanHo = canHo.LoaiCanHoId.Name,
            TinhTrangCanHoId = canHo.TinhTrangCanHoId.Value,
            TenTinhTrangCanHo = canHo.TinhTrangCanHoId.Name
        });
    }
}
