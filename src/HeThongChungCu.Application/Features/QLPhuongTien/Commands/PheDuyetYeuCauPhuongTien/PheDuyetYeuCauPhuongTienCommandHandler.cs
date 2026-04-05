using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Application.Features.UploadMedia.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.PheDuyetYeuCauPhuongTien;

public class PheDuyetYeuCauPhuongTienCommandHandler : ICommandHandler<PheDuyetYeuCauPhuongTienCommand, YeuCauPhuongTienResponse>
{
    private readonly IYeuCauPhuongTienCommandRepository _yeuCauRepository;
    private readonly IPhuongTienCommandRepository _phuongTienRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly IToaNhaCommandRepository _toaNhaCommandRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IVehicleRegistryService _vehicleRegistryService;
    private readonly IDocumentReconciliationService _documentReconciliationService;
    private readonly IUnitOfWork _unitOfWork;

    public PheDuyetYeuCauPhuongTienCommandHandler(
        IYeuCauPhuongTienCommandRepository yeuCauRepository,
        IPhuongTienCommandRepository phuongTienRepository,
        ICanHoCommandRepository canHoRepository,
        INguoiDungCommandRepository nguoiDungRepository,
        IToaNhaCommandRepository toaNhaCommandRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IVehicleRegistryService vehicleRegistryService,
        IDocumentReconciliationService documentReconciliationService,
        IUnitOfWork unitOfWork)
    {
        _yeuCauRepository = yeuCauRepository;
        _phuongTienRepository = phuongTienRepository;
        _canHoRepository = canHoRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _toaNhaCommandRepository = toaNhaCommandRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _vehicleRegistryService = vehicleRegistryService;
        _documentReconciliationService = documentReconciliationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauPhuongTienResponse>> Handle(PheDuyetYeuCauPhuongTienCommand request, CancellationToken cancellationToken)
    {
        var adminId = _currentUserService.UserId;
        if (adminId == null)
            return Result.Failure<YeuCauPhuongTienResponse>(UserErrors.NotFound);

        var yeuCau = await _yeuCauRepository.GetByIdAsync(request.YeuCauPhuongTienId, cancellationToken);
        if (yeuCau == null)
            return Result.Failure<YeuCauPhuongTienResponse>(YeuCauPhuongTienErrors.NotFound);

        if (yeuCau.TrangThaiId != TrangThaiYeuCau.Pending)
            return Result.Failure<YeuCauPhuongTienResponse>(new Error("YeuCauPhuongTien.InvalidStatus", "Chỉ có thể duyệt yêu cầu đang chờ duyệt."));

        var now = _dateTimeProvider.UtcNow;
        yeuCau.Approve(adminId.Value, now);

        if (yeuCau.LoaiYeuCauId == LoaiYeuCau.Them)
        {
            var canHo = await _canHoRepository.GetByIdAsync(yeuCau.CanHoId, cancellationToken);
            if (canHo == null)
                return Result.Failure<YeuCauPhuongTienResponse>(CanHoErrors.NotFoundById(yeuCau.CanHoId));

            // Gather data for Domain Service
            var activeVehicles = await _phuongTienRepository.GetPhuongTiensByCanHoIdAsync(yeuCau.CanHoId, cancellationToken);
            var isPlateDuplicate = await _phuongTienRepository.BienSoExistsAsync(yeuCau.YeuCauBienSo, cancellationToken);

            // Delegate to Domain Service
            var validationResult = _vehicleRegistryService.CanRegisterOrUpdateVehicle(
                canHo,
                yeuCau.YeuCauLoaiPhuongTienId,
                activeVehicles.Where(v => v.TrangThaiPhuongTienId == TrangThaiPhuongTien.Active),
                isPlateDuplicate);

            if (validationResult.IsFailure)
                return Result.Failure<YeuCauPhuongTienResponse>(validationResult.Errors[0]);

            var phuongTien = new PhuongTien(
                yeuCau.CanHoId,
                yeuCau.YeuCauTenPhuongTien,
                yeuCau.YeuCauLoaiPhuongTienId,
                yeuCau.YeuCauBienSo,
                yeuCau.YeuCauMauXe,
                yeuCau.YeuCauHinhAnhPhuongTiens.Select(f => new TepPhuongTien(f.FileName, f.FileUrl, f.Size, f.ContentType)).ToList());

            await _phuongTienRepository.AddAsync(phuongTien, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else if (yeuCau.LoaiYeuCauId == LoaiYeuCau.Sua)
        {
            var phuongTien = await _phuongTienRepository.GetPhuongTienByIdAsync(yeuCau.YeuCauPhuongTienId!.Value, cancellationToken);
            if (phuongTien == null)
                return Result.Failure<YeuCauPhuongTienResponse>(PhuongTienErrors.NotFound);

            // Gather data for Domain Service
            var canHo = await _canHoRepository.GetByIdAsync(yeuCau.CanHoId, cancellationToken);
            var activeVehicles = await _phuongTienRepository.GetPhuongTiensByCanHoIdAsync(yeuCau.CanHoId, cancellationToken);
            
            bool isPlateDuplicate = false;
            if (phuongTien.BienSo != yeuCau.YeuCauBienSo)
            {
                isPlateDuplicate = await _phuongTienRepository.BienSoExistsAsync(yeuCau.YeuCauBienSo, cancellationToken);
            }

            // Delegate to Domain Service (handles both Quota and Uniqueness)
            var validationResult = _vehicleRegistryService.CanRegisterOrUpdateVehicle(
                canHo!,
                yeuCau.YeuCauLoaiPhuongTienId,
                activeVehicles.Where(v => v.TrangThaiPhuongTienId == TrangThaiPhuongTien.Active),
                isPlateDuplicate,
                phuongTien.Id);

            if (validationResult.IsFailure)
                return Result.Failure<YeuCauPhuongTienResponse>(validationResult.Errors[0]);

            phuongTien.CapNhat(
                yeuCau.YeuCauTenPhuongTien,
                yeuCau.YeuCauLoaiPhuongTienId,
                yeuCau.YeuCauBienSo,
                yeuCau.YeuCauMauXe);

            _documentReconciliationService.ReconcilePhuongTienImages(phuongTien, yeuCau.YeuCauHinhAnhPhuongTiens);

            _phuongTienRepository.Update(phuongTien);
        }
        else if (yeuCau.LoaiYeuCauId == LoaiYeuCau.Xoa)
        {
            var phuongTien = await _phuongTienRepository.GetPhuongTienByIdAsync(yeuCau.YeuCauPhuongTienId!.Value, cancellationToken);
            if (phuongTien != null)
            {
                phuongTien.Huy(now.DateTime);
                _phuongTienRepository.Update(phuongTien);
            }
        }

        _yeuCauRepository.Update(yeuCau);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var sender = await _nguoiDungRepository.GetByIdAsync(yeuCau.CreatedBy, cancellationToken);
        var canHoResponse = await _canHoRepository.GetByIdAsync(yeuCau.CanHoId, cancellationToken);
        var toaNha = await _toaNhaCommandRepository.GetToaNhaByTangIdAsync(canHoResponse!.TangId, cancellationToken);
        var tang = toaNha!.Tangs.First(t => t.Id == canHoResponse.TangId);

        var processor = await _nguoiDungRepository.GetByIdAsync(adminId.Value, cancellationToken);

        return Result.Success(new YeuCauPhuongTienResponse
        {
            Id = yeuCau.Id,
            CreatedBy = yeuCau.CreatedBy,
            TenNguoiGui = $"{sender!.Ho} {sender.Ten}".Trim(),
            CreatedAt = yeuCau.CreatedAt,
            CanHoId = yeuCau.CanHoId,
            TenCanHo = canHoResponse.MaCanHo,
            TenTang = tang.MaTang,
            TenToaNha = toaNha.MaToaNha,
            PhuongTienId = yeuCau.YeuCauPhuongTienId,
            LoaiYeuCauId = yeuCau.LoaiYeuCauId.Value,
            TenLoaiYeuCau = yeuCau.LoaiYeuCauId.Name,
            TrangThaiId = yeuCau.TrangThaiId.Value,
            TenTrangThai = yeuCau.TrangThaiId.Name,
            NoiDung = yeuCau.NoiDung,
            LyDo = yeuCau.LyDo,
            NguoiXuLyId = yeuCau.NguoiXuLyId,
            TenNguoiXuLy = $"{processor!.Ho} {processor.Ten}".Trim(),
            NgayXuLy = yeuCau.NgayXuLy,
            YeuCauTenPhuongTien = yeuCau.YeuCauTenPhuongTien,
            YeuCauLoaiPhuongTienId = yeuCau.YeuCauLoaiPhuongTienId.Value,
            TenYeuCauLoaiPhuongTien = yeuCau.YeuCauLoaiPhuongTienId.Name,
            YeuCauBienSo = yeuCau.YeuCauBienSo,
            YeuCauMauXe = yeuCau.YeuCauMauXe,
            YeuCauHinhAnhPhuongTiens = yeuCau.YeuCauHinhAnhPhuongTiens.Select(f => new TepTaiLieuResponse(
                f.Id,
                f.FileUrl,
                f.FileName,
                f.ContentType)).ToList()
        });
    }
}
