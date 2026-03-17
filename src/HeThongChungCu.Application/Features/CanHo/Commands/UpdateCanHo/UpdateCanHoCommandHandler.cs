using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Application.Features.Tang.DTOs;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Policies;

namespace HeThongChungCu.Application.Features.CanHo.Commands.UpdateCanHo;

public class UpdateCanHoCommandHandler : ICommandHandler<UpdateCanHoCommand, CanHoDetailResponse>
{
    private readonly ICanHoEFRepository _canHoRepository;
    private readonly ITangEFRepository _tangRepository;
    private readonly IQuanHeCuTruEFRepository _quanHeCuTruRepository;
    private readonly ICanHoPolicy _canHoPolicy;

    public UpdateCanHoCommandHandler(
        ICanHoEFRepository canHoRepository,
        ITangEFRepository tangRepository,
        IQuanHeCuTruEFRepository quanHeCuTruRepository,
        ICanHoPolicy canHoPolicy)
    {
        _canHoRepository = canHoRepository;
        _tangRepository = tangRepository;
        _quanHeCuTruRepository = quanHeCuTruRepository;
        _canHoPolicy = canHoPolicy;
    }

    public async Task<Result<CanHoDetailResponse>> Handle(UpdateCanHoCommand request, CancellationToken cancellationToken)
    {
        var tang = await _tangRepository.GetByIdAsync(request.TangId, cancellationToken);
        if (tang is null)
            return Result.Failure<CanHoDetailResponse>(CanHoErrors.NotFoundById(request.TangId));

        var canHo = await _canHoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (canHo is null || canHo.TangId != request.TangId)
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
        bool hasActiveResidents = relations.Any(r => !r.IsKetThuc);

        canHo.UpdateInfo(
            request.TenCanHo, 
            request.MaCanHo,
            request.DienTich, 
            request.SoPhongNgu, 
            request.SoPhongTam, 
            loaiCanHo!,
            _canHoPolicy,
            hasActiveResidents);

        canHo.UpdateStatus(tinhTrangCanHo!, _canHoPolicy);

        _canHoRepository.Update(canHo);

        return Result.Success(new CanHoDetailResponse
        {
            Id = canHo.Id,
            MaCanHo = canHo.MaCanHo,
            TenCanHo = canHo.TenCanHo,
            DienTich = canHo.DienTich,
            TangId = canHo.TangId,
            TenTang = canHo.Tang?.TenTang ?? string.Empty,
            SoPhongNgu = canHo.SoPhongNgu,
            SoPhongTam = canHo.SoPhongTam,
            LoaiCanHoId = canHo.LoaiCanHoId.Value,
            TenLoaiCanHo = canHo.LoaiCanHoId.Name,
            TinhTrangCanHoId = canHo.TinhTrangCanHoId.Value,
            TenTinhTrangCanHo = canHo.TinhTrangCanHoId.Name
        });
    }
}
