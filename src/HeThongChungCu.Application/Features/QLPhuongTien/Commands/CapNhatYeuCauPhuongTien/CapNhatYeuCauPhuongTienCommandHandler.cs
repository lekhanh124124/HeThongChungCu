using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.CapNhatYeuCauPhuongTien;

public class CapNhatYeuCauPhuongTienCommandHandler : ICommandHandler<CapNhatYeuCauPhuongTienCommand, YeuCauPhuongTienResponse>
{
    private readonly IYeuCauPhuongTienEFRepository _yeuCauRepository;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CapNhatYeuCauPhuongTienCommandHandler(
        IYeuCauPhuongTienEFRepository yeuCauRepository,
        ITepTaiLieuRepository tepTaiLieuRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _yeuCauRepository = yeuCauRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
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
            List<TepTaiLieu>? images = null;
            if (request.FileIds != null)
            {
                var tepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(request.FileIds, cancellationToken);
                images = tepTaiLieus.ToList();
            }

            var loaiPhuongTien = request.LoaiPhuongTienId.HasValue 
                ? LoaiPhuongTien.FromValue(request.LoaiPhuongTienId.Value, null) 
                : null;

            yeuCau.Update(
                loaiPhuongTien,
                request.TenPhuongTien,
                request.BienSo,
                request.MauXe,
                request.NoiDung,
                images);

            if (request.IsSubmit)
            {
                yeuCau.Submit();
            }
        }

        _yeuCauRepository.Update(yeuCau);
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
