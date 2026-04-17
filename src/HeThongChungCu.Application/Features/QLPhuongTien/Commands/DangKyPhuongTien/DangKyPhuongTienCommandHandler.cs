using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Application.Features.UploadMedia.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.DangKyPhuongTien;

internal sealed class DangKyPhuongTienCommandHandler : ICommandHandler<DangKyPhuongTienCommand, PhuongTienResponse>
{
    private readonly IPhuongTienCommandRepository _phuongTienCommandRepository;
    private readonly ICanHoCommandRepository _canHoCommandRepository;
    private readonly IToaNhaCommandRepository _toaNhaCommandRepository;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepository;
    private readonly IVehicleRegistryService _vehicleRegistryService;
    private readonly IUnitOfWork _unitOfWork;

    public DangKyPhuongTienCommandHandler(
        IPhuongTienCommandRepository phuongTienCommandRepository,
        ICanHoCommandRepository canHoCommandRepository,
        IToaNhaCommandRepository toaNhaCommandRepository,
        ITepTaiLieuCommandRepository tepTaiLieuRepository,
        IVehicleRegistryService vehicleRegistryService,
        IUnitOfWork unitOfWork)
    {
        _phuongTienCommandRepository = phuongTienCommandRepository;
        _canHoCommandRepository = canHoCommandRepository;
        _toaNhaCommandRepository = toaNhaCommandRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _vehicleRegistryService = vehicleRegistryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PhuongTienResponse>> Handle(DangKyPhuongTienCommand request, CancellationToken cancellationToken)
    {
        var canHo = await _canHoCommandRepository.GetByIdAsync(request.CanHoId, cancellationToken);
        if (canHo == null)
            return Result.Failure<PhuongTienResponse>(CanHoErrors.NotFoundById(request.CanHoId));

        var toaNha = await _toaNhaCommandRepository.GetToaNhaByTangIdAsync(canHo.TangId, cancellationToken);
        if (toaNha == null)
            return Result.Failure<PhuongTienResponse>(CanHoErrors.NotFoundById(request.CanHoId));

        var tang = toaNha.Tangs.FirstOrDefault(t => t.Id == canHo.TangId);
        if (tang == null)
            return Result.Failure<PhuongTienResponse>(CanHoErrors.NotFoundById(request.CanHoId));

        var loaiPhuongTien = LoaiPhuongTien.FromValue(request.LoaiPhuongTienId)!;
        
        // Gather data for Domain Service
        var existingVehicles = await _phuongTienCommandRepository.GetPhuongTiensByCanHoIdAsync(request.CanHoId, cancellationToken);
        var isPlateDuplicate = await _phuongTienCommandRepository.BienSoExistsAsync(request.BienSo, cancellationToken);

        // Delegate to Domain Service
        var validationResult = _vehicleRegistryService.CanRegisterOrUpdateVehicle(
            canHo,
            loaiPhuongTien,
            existingVehicles.Where(v => v.TrangThaiPhuongTienId == TrangThaiPhuongTien.Active),
            isPlateDuplicate);

        if (validationResult.IsFailure)
            return Result.Failure<PhuongTienResponse>(validationResult.Errors[0]);

        IEnumerable<TepTaiLieu>? hinhAnhs = null;
        if (request.HinhAnhIds != null && request.HinhAnhIds.Any())
        {
            hinhAnhs = await _tepTaiLieuRepository.GetByIdsAsync(request.HinhAnhIds, cancellationToken);
        }

        var phuongTienEntity = new PhuongTien(
            request.CanHoId,
            request.TenPhuongTien,
            LoaiPhuongTien.FromValue(request.LoaiPhuongTienId)!,
            request.BienSo,
            request.MauXe,
            hinhAnhs?.Select(f => new TepPhuongTien(f.FileName, f.FileUrl, f.Size, f.ContentType)).ToList());

        var phuongTien = await _phuongTienCommandRepository.AddAsync(phuongTienEntity, cancellationToken);

        // TransactionBehavior will automatically commit if no exception is thrown, otherwise it will rollback

        return Result.Success(new PhuongTienResponse
        {
            Id = phuongTien.Id,
            CanHoId = phuongTien.CanHoId,
            MaToaNha = toaNha.MaToaNha,
            MaTang = tang.MaTang,
            MaCanHo = canHo.MaCanHo,
            BienSo = phuongTien.BienSo,
            LoaiPhuongTienId = phuongTien.LoaiPhuongTienId.Value,
            TenTrangThaiPhuongTien= phuongTien.TrangThaiPhuongTienId.Name,
            MauXe = phuongTien.MauXe,
            TenPhuongTien = phuongTien.TenPhuongTien,
            TrangThaiPhuongTienId = phuongTien.TrangThaiPhuongTienId.Value,
            TenLoaiPhuongTien = phuongTien.LoaiPhuongTienId.Name,
            ThePhuongTiens = phuongTien.ThePhuongTiens.Select(x => new ThePhuongTienResponse
            {
                Id = x.Id,
                PhuongTienId = x.PhuongTienId,
                MaThe = x.MaThe,
                NgayBatDau = x.ThoiGian.NgayBatDau,
                NgayKetThuc = x.ThoiGian.NgayKetThuc,
                TrangThaiThePhuongTienId = x.TrangThaiId.Value,
                TenTrangThaiThePhuongTien = x.TrangThaiId.Name
             }).ToList(),
            HinhAnhPhuongTiens = phuongTien.HinhAnhPhuongTiens.Select(x => new UploadFileResponse(
                x.Id,
                x.FileName,
                x.FileUrl,
                x.ContentType)).ToList()
        });
    }
}
