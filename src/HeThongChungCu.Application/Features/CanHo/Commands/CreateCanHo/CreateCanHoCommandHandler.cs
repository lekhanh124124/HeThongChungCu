using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Application.Features.CanHo.Commands.CreateCanHo;

public class CreateCanHoCommandHandler : ICommandHandler<CreateCanHoCommand, CanHoDetailResponse>
{
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly ICanHoDomainService _canHoDomainService;

    public CreateCanHoCommandHandler(
        ICanHoCommandRepository canHoRepository,
        IToaNhaCommandRepository toaNhaRepository,
        ICanHoDomainService canHoDomainService)
    {
        _canHoRepository = canHoRepository;
        _toaNhaRepository = toaNhaRepository;
        _canHoDomainService = canHoDomainService;
    }
    public async Task<Result<CanHoDetailResponse>> Handle(CreateCanHoCommand request, CancellationToken cancellationToken)
    {
        var toaNha = await _toaNhaRepository.GetToaNhaByTangIdAsync(request.TangId, cancellationToken);
        if (toaNha == null)
            return ToaNhaErrors.NotFound;

        var tang = toaNha.Tangs.FirstOrDefault(t => t.Id == request.TangId);
        if (tang == null)
            return TangErrors.NotFound;

        var maExists = await _canHoRepository.MaCanHoExistsAsync(request.MaCanHo, cancellationToken);
        
        var canCreateResult = _canHoDomainService.CanCreateCanHo(tang, request.MaCanHo, maExists);
        if (canCreateResult.IsFailure)
            return canCreateResult.Errors;

        var loaiCanHo = LoaiCanHo.FromValue(request.LoaiCanHoId);
        var tinhTrangCanHo = TrangThaiCanHo.DangTrong;

        var canHo = Domain.Entities.CanHo.Create(
            tang.Id,
            request.MaCanHo, 
            request.TenCanHo, 
            request.DienTich, 
            request.SoPhongNgu,
            request.SoPhongTam, 
            loaiCanHo!, 
            tinhTrangCanHo);

        await _canHoRepository.AddAsync(canHo, cancellationToken);
        return Result.Success(new CanHoDetailResponse
        {
            Id = canHo!.Id,
            MaCanHo = canHo.MaCanHo,
            TenCanHo = canHo.TenCanHo,
            DienTich = canHo.ThongSo.DienTich,
            TangId = canHo.TangId,
            TenTang = tang.TenTang,
            SoPhongNgu = canHo.ThongSo.SoPhongNgu,
            SoPhongTam = canHo.ThongSo.SoPhongTam,
            LoaiCanHoId = canHo.LoaiCanHoId.Value,
            TenLoaiCanHo = canHo.LoaiCanHoId.Name,
            TinhTrangCanHoId = canHo.TinhTrangCanHoId.Value,
            TenTinhTrangCanHo = canHo.TinhTrangCanHoId.Name
        });
    }
}
