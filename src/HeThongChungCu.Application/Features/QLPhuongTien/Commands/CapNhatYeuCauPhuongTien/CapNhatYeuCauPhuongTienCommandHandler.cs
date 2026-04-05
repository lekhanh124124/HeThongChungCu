using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.CapNhatYeuCauPhuongTien;

public class CapNhatYeuCauPhuongTienCommandHandler : ICommandHandler<CapNhatYeuCauPhuongTienCommand, YeuCauPhuongTienResponse>
{
    private readonly IYeuCauPhuongTienCommandRepository _yeuCauRepository;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly ICanHoCommandRepository _canHoCommandRepository;
    private readonly IToaNhaCommandRepository _toaNhaCommandRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CapNhatYeuCauPhuongTienCommandHandler(
        IYeuCauPhuongTienCommandRepository yeuCauRepository,
        ITepTaiLieuRepository tepTaiLieuRepository,
        INguoiDungCommandRepository nguoiDungRepository,
        ICanHoCommandRepository canHoCommandRepository,
        IToaNhaCommandRepository toaNhaCommandRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _yeuCauRepository = yeuCauRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _canHoCommandRepository = canHoCommandRepository;
        _toaNhaCommandRepository = toaNhaCommandRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauPhuongTienResponse>> Handle(CapNhatYeuCauPhuongTienCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Result.Failure<YeuCauPhuongTienResponse>(UserErrors.NotFound);

        var yeuCau = await _yeuCauRepository.GetByIdAsync(request.Id, cancellationToken);
        if (yeuCau == null)
            return Result.Failure<YeuCauPhuongTienResponse>(YeuCauPhuongTienErrors.NotFound);

        if (yeuCau.CreatedBy != userId)
            return Result.Failure<YeuCauPhuongTienResponse>(YeuCauPhuongTienErrors.Forbidden);

        if (request.IsWithdraw)
        {
            yeuCau.Withdraw();
        }
        else
        {
            // Fetch TepTaiLieus if provided
            List<TepYeuCauPhuongTien>? images = null;
            if (request.FileIds != null)
            {
                var tepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(request.FileIds, cancellationToken);
                images = tepTaiLieus.Select(f => f is TepYeuCauPhuongTien tp ? tp : new TepYeuCauPhuongTien(f.FileName, f.FileUrl, f.Size, f.ContentType)).ToList();
            }

            var loaiPhuongTien = request.LoaiPhuongTienId.HasValue 
                ? LoaiPhuongTien.FromValue(request.LoaiPhuongTienId.Value, null) 
                : null;

            yeuCau.Update(
                loaiPhuongTien,
                request.YeuCauTenPhuongTien,
                request.YeuCauBienSo,
                request.YeuCauMauXe,
                request.NoiDung,
                images);

            if (request.IsSubmit)
            {
                yeuCau.Submit();
            }
        }

        _yeuCauRepository.Update(yeuCau);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var sender = await _nguoiDungRepository.GetByIdAsync(userId.Value, cancellationToken);
        var canHo = await _canHoCommandRepository.GetByIdAsync(yeuCau.CanHoId, cancellationToken);
        var toaNha = await _toaNhaCommandRepository.GetToaNhaByTangIdAsync(canHo!.TangId, cancellationToken);
        var tang = toaNha!.Tangs.First(t => t.Id == canHo.TangId);
        
        string? processorName = null;
        if (yeuCau.NguoiXuLyId.HasValue)
        {
            var processor = await _nguoiDungRepository.GetByIdAsync(yeuCau.NguoiXuLyId.Value, cancellationToken);
             processorName = processor != null ? $"{processor.Ho} {processor.Ten}".Trim() : null;
        }

        return new YeuCauPhuongTienResponse
        {
            Id = yeuCau.Id,
            CreatedBy = yeuCau.CreatedBy,
            TenNguoiGui = $"{sender!.Ho} {sender.Ten}".Trim(),
            CreatedAt = yeuCau.CreatedAt,
            CanHoId = yeuCau.CanHoId,
            TenCanHo = canHo.MaCanHo,
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
            TenNguoiXuLy = processorName,
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
        };
    }
}
