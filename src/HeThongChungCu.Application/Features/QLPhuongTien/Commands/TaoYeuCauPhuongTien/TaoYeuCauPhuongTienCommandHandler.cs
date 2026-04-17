using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TaoYeuCauPhuongTien;

public class TaoYeuCauPhuongTienCommandHandler : ICommandHandler<TaoYeuCauPhuongTienCommand, YeuCauPhuongTienResponse>
{
    private readonly IYeuCauPhuongTienCommandRepository _yeuCauRepository;
    private readonly IPhuongTienCommandRepository _phuongTienRepository;
    private readonly IQuanHeCuTruCommandRepository _quanHeRepository;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly ICanHoCommandRepository _canHoCommandRepository;
    private readonly IToaNhaCommandRepository _toaNhaCommandRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public TaoYeuCauPhuongTienCommandHandler(
        IYeuCauPhuongTienCommandRepository yeuCauRepository,
        IPhuongTienCommandRepository phuongTienRepository,
        IQuanHeCuTruCommandRepository quanHeRepository,
        ITepTaiLieuRepository tepTaiLieuRepository,
        INguoiDungCommandRepository nguoiDungRepository,
        ICanHoCommandRepository canHoCommandRepository,
        IToaNhaCommandRepository toaNhaCommandRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _yeuCauRepository = yeuCauRepository;
        _phuongTienRepository = phuongTienRepository;
        _quanHeRepository = quanHeRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _canHoCommandRepository = canHoCommandRepository;
        _toaNhaCommandRepository = toaNhaCommandRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauPhuongTienResponse>> Handle(TaoYeuCauPhuongTienCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Result.Failure<YeuCauPhuongTienResponse>(UserErrors.NotFound);

        // Fetch the relation of the current user for this apartment to validate ChuHo
        var requesterRelation = await _quanHeRepository.GetByUserAndCanHoAsync(userId.Value, request.CanHoId, cancellationToken);
        if (requesterRelation == null)
            return Result.Failure<YeuCauPhuongTienResponse>(CanHoErrors.NotFoundById(request.CanHoId));

        if (requesterRelation.LoaiQuanHeCuTruId != LoaiQuanHeCuTru.ChuHo)
            return Result.Failure<YeuCauPhuongTienResponse>(YeuCauPhuongTienErrors.Forbidden);

        var loaiYeuCau = LoaiHanhDongYeuCau.FromValue(request.LoaiYeuCauId, null);

        // Fetch all TepTaiLieus at once
        var tepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(request.FileIds ?? new List<int>(), cancellationToken);

        var initialStatus = request.IsSubmit ? TrangThaiYeuCau.Pending : TrangThaiYeuCau.Saved;

        YeuCauPhuongTien yeuCau;
        var now = _dateTimeProvider.Now;

        if (loaiYeuCau == LoaiHanhDongYeuCau.Them)
        {
            var loaiPhuongTien = LoaiPhuongTien.FromValue(request.YeuCauLoaiPhuongTienId!.Value, null);
            yeuCau = YeuCauPhuongTien.CreateAddRequest(
                request.CanHoId,
                loaiPhuongTien!,
                request.YeuCauTenPhuongTien!,
                request.YeuCauBienSo!,
                request.YeuCauMauXe!,
                request.NoiDung,
                tepTaiLieus.Select(f => f is TepYeuCauPhuongTien tp ? tp : new TepYeuCauPhuongTien(f.FileName, f.FileUrl, f.Size, f.ContentType)).ToList(),
                initialStatus);
        }
        else // Sua hoặc Xoa
        {
            var phuongTien = await _phuongTienRepository.GetPhuongTienByIdAsync(request.YeuCauPhuongTienId!.Value, cancellationToken);
            if (phuongTien == null || phuongTien.CanHoId != request.CanHoId)
                return Result.Failure<YeuCauPhuongTienResponse>(PhuongTienErrors.NotFound);

            if (loaiYeuCau == LoaiHanhDongYeuCau.Sua)
            {
                var loaiPhuongTien = request.YeuCauLoaiPhuongTienId.HasValue
                    ? LoaiPhuongTien.FromValue(request.YeuCauLoaiPhuongTienId.Value, null)!
                    : phuongTien.LoaiPhuongTienId;

                yeuCau = YeuCauPhuongTien.CreateUpdateRequest(
                    request.CanHoId,
                    phuongTien.Id,
                    loaiPhuongTien,
                    request.YeuCauTenPhuongTien ?? phuongTien.TenPhuongTien,
                    request.YeuCauBienSo ?? phuongTien.BienSo,
                    request.YeuCauMauXe ?? phuongTien.MauXe,
                    request.NoiDung,
                    tepTaiLieus.Select(f => f is TepYeuCauPhuongTien tp ? tp : new TepYeuCauPhuongTien(f.FileName, f.FileUrl, f.Size, f.ContentType)).ToList(),
                    initialStatus);
            }
            else // Xoa
            {
                yeuCau = YeuCauPhuongTien.CreateDeleteRequest(
                    request.CanHoId,
                    phuongTien.Id,
                    phuongTien.LoaiPhuongTienId,
                    phuongTien.TenPhuongTien,
                    phuongTien.BienSo,
                    phuongTien.MauXe,
                    request.NoiDung,
                    initialStatus);
            }
        }

        await _yeuCauRepository.AddAsync(yeuCau, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var sender = await _nguoiDungRepository.GetByIdAsync(userId.Value, cancellationToken);
        var canHo = await _canHoCommandRepository.GetByIdAsync(request.CanHoId, cancellationToken);
        var toaNha = await _toaNhaCommandRepository.GetToaNhaByTangIdAsync(canHo!.TangId, cancellationToken);
        var tang = toaNha!.Tangs.First(t => t.Id == canHo.TangId);

        return new YeuCauPhuongTienResponse
        {
            Id = yeuCau.Id,
            CreatedBy = userId.Value,
            TenNguoiGui = $"{sender!.Ho} {sender.Ten}".Trim(),
            CreatedAt = yeuCau.CreatedAt,
            CanHoId = yeuCau.CanHoId,
            TenCanHo = canHo.MaCanHo,
            TenTang = tang.MaTang,
            TenToaNha = toaNha.MaToaNha,
            PhuongTienId = yeuCau.YeuCauPhuongTienId,
            LoaiYeuCauId = yeuCau.LoaiHanhDongYeuCauId.Value,
            TenLoaiYeuCau = yeuCau.LoaiHanhDongYeuCauId.Name,
            TrangThaiId = yeuCau.TrangThaiId.Value,
            TenTrangThai = yeuCau.TrangThaiId.Name,
            NoiDung = yeuCau.NoiDung,
            LyDo = yeuCau.LyDo,
            NguoiXuLyId = yeuCau.NguoiXuLyId,
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
