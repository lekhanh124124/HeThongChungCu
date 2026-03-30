using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TaoYeuCauPhuongTien;

public class TaoYeuCauPhuongTienCommandHandler : ICommandHandler<TaoYeuCauPhuongTienCommand, YeuCauPhuongTienResponse>
{
    private readonly IYeuCauPhuongTienEFRepository _yeuCauRepository;
    private readonly IPhuongTienEFRepository _phuongTienRepository;
    private readonly IQuanHeCuTruEFRepository _quanHeRepository;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public TaoYeuCauPhuongTienCommandHandler(
        IYeuCauPhuongTienEFRepository yeuCauRepository,
        IPhuongTienEFRepository phuongTienRepository,
        IQuanHeCuTruEFRepository quanHeRepository,
        ITepTaiLieuRepository tepTaiLieuRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _yeuCauRepository = yeuCauRepository;
        _phuongTienRepository = phuongTienRepository;
        _quanHeRepository = quanHeRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
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

        var loaiYeuCau = LoaiYeuCau.FromValue(request.LoaiYeuCauId, null);

        // Fetch all TepTaiLieus at once
        var tepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(request.FileIds ?? new List<int>(), cancellationToken);

        YeuCauPhuongTien yeuCau;
        var now = _dateTimeProvider.Now;

        if (loaiYeuCau == LoaiYeuCau.Them)
        {
            var loaiPhuongTien = LoaiPhuongTien.FromValue(request.LoaiPhuongTienId!.Value, null);
            yeuCau = YeuCauPhuongTien.CreateAddRequest(
                request.CanHoId,
                loaiPhuongTien!,
                request.TenPhuongTien!,
                request.BienSo!,
                request.MauXe!,
                request.NoiDung,
                tepTaiLieus);
        }
        else // Sua hoặc Xoa
        {
            var phuongTien = await _phuongTienRepository.GetPhuongTienByIdAsync(request.PhuongTienId!.Value, cancellationToken);
            if (phuongTien == null || phuongTien.CanHoId != request.CanHoId)
                return Result.Failure<YeuCauPhuongTienResponse>(PhuongTienErrors.NotFound);

            if (loaiYeuCau == LoaiYeuCau.Sua)
            {
                var loaiPhuongTien = request.LoaiPhuongTienId.HasValue
                    ? LoaiPhuongTien.FromValue(request.LoaiPhuongTienId.Value, null)!
                    : phuongTien.LoaiPhuongTienId;

                yeuCau = YeuCauPhuongTien.CreateUpdateRequest(
                    request.CanHoId,
                    phuongTien.Id,
                    loaiPhuongTien,
                    request.TenPhuongTien ?? phuongTien.TenPhuongTien,
                    request.BienSo ?? phuongTien.BienSo,
                    request.MauXe ?? phuongTien.MauXe,
                    request.NoiDung,
                    tepTaiLieus);
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
                    request.NoiDung);
            }
        }

        await _yeuCauRepository.AddAsync(yeuCau, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new YeuCauPhuongTienResponse
        {
            Id = yeuCau.Id,
            CanHoId = yeuCau.CanHoId,
            PhuongTienId = yeuCau.YeuCauPhuongTienId,
            LoaiYeuCauId = yeuCau.LoaiYeuCauId.Value,
            TenLoaiYeuCau = yeuCau.LoaiYeuCauId.Name,
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
            CreatedAt = yeuCau.CreatedAt,
            YeuCauHinhAnhPhuongTiens = yeuCau.YeuCauHinhAnhPhuongTiens.Select(f => new TepTaiLieuResponse(
                f.Id,
                f.FileUrl,
                f.FileName,
                f.ContentType)).ToList()
        };
    }
}
