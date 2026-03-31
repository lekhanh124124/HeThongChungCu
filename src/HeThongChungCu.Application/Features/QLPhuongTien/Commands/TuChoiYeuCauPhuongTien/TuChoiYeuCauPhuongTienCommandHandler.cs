using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TuChoiYeuCauPhuongTien;

public class TuChoiYeuCauPhuongTienCommandHandler : ICommandHandler<TuChoiYeuCauPhuongTienCommand, YeuCauPhuongTienResponse>
{
    private readonly IYeuCauPhuongTienEFRepository _yeuCauRepository;
    private readonly INguoiDungEFRepository _nguoiDungRepository;
    private readonly ICanHoEFRepository _canHoEFRepository;
    private readonly IToaNhaEFRepository _toaNhaEFRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public TuChoiYeuCauPhuongTienCommandHandler(
        IYeuCauPhuongTienEFRepository yeuCauRepository,
        INguoiDungEFRepository nguoiDungRepository,
        ICanHoEFRepository canHoEFRepository,
        IToaNhaEFRepository toaNhaEFRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _yeuCauRepository = yeuCauRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _canHoEFRepository = canHoEFRepository;
        _toaNhaEFRepository = toaNhaEFRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauPhuongTienResponse>> Handle(TuChoiYeuCauPhuongTienCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Result.Failure<YeuCauPhuongTienResponse>(UserErrors.NotFound);

        var yeuCau = await _yeuCauRepository.GetByIdAsync(request.YeuCauPhuongTienId, cancellationToken);
        if (yeuCau == null)
            return Result.Failure<YeuCauPhuongTienResponse>(YeuCauPhuongTienErrors.NotFound);

        if (yeuCau.TrangThaiId != TrangThaiYeuCau.Pending)
            return Result.Failure<YeuCauPhuongTienResponse>(new Error("YeuCauPhuongTien.InvalidStatus", "Chỉ có thể từ chối yêu cầu đang chờ duyệt."));

        var now = _dateTimeProvider.UtcNow;
        yeuCau.Reject(userId.Value, request.LyDo, now);
        _yeuCauRepository.Update(yeuCau);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var sender = await _nguoiDungRepository.GetByIdAsync(yeuCau.CreatedBy, cancellationToken);
        var canHo = await _canHoEFRepository.GetByIdAsync(yeuCau.CanHoId, cancellationToken);
        var toaNha = await _toaNhaEFRepository.GetToaNhaByTangIdAsync(canHo!.TangId, cancellationToken);
        var tang = toaNha!.Tangs.First(t => t.Id == canHo.TangId);

        var processor = await _nguoiDungRepository.GetByIdAsync(userId.Value, cancellationToken);

        return Result.Success(new YeuCauPhuongTienResponse
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
