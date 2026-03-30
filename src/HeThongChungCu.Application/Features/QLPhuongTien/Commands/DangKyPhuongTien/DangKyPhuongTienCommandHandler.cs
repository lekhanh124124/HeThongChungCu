using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Application.Features.UploadMedia.DTOs;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.DangKyPhuongTien;

internal sealed class DangKyPhuongTienCommandHandler : ICommandHandler<DangKyPhuongTienCommand, PhuongTienResponse>
{
    private readonly IPhuongTienEFRepository _phuongTienEFRepository;
    private readonly ICanHoEFRepository _canHoEFRepository;
    private readonly IToaNhaEFRepository _toaNhaEFRepository;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DangKyPhuongTienCommandHandler(
        IPhuongTienEFRepository phuongTienEFRepository,
        ICanHoEFRepository canHoEFRepository,
        IToaNhaEFRepository toaNhaEFRepository,
        ITepTaiLieuRepository tepTaiLieuRepository,
        IUnitOfWork unitOfWork)
    {
        _phuongTienEFRepository = phuongTienEFRepository;
        _canHoEFRepository = canHoEFRepository;
        _toaNhaEFRepository = toaNhaEFRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PhuongTienResponse>> Handle(DangKyPhuongTienCommand request, CancellationToken cancellationToken)
    {
        var canHo = await _canHoEFRepository.GetByIdAsync(request.CanHoId, cancellationToken);
        if (canHo == null)
            return Result.Failure<PhuongTienResponse>(CanHoErrors.NotFoundById(request.CanHoId));

        var toaNha = await _toaNhaEFRepository.GetToaNhaByTangIdAsync(canHo.TangId, cancellationToken);
        if (toaNha == null)
            return Result.Failure<PhuongTienResponse>(CanHoErrors.NotFoundById(request.CanHoId));

        var tang = toaNha.Tangs.FirstOrDefault(t => t.Id == canHo.TangId);
        if (tang == null)
            return Result.Failure<PhuongTienResponse>(CanHoErrors.NotFoundById(request.CanHoId));

        var bienSoExists = await _phuongTienEFRepository.BienSoExistsAsync(request.BienSo, cancellationToken);
        if (bienSoExists)
            return Result.Failure<PhuongTienResponse>(PhuongTienErrors.BienSoExists);

        // Kiểm tra hạn mức ngay khi đăng ký (Cảnh báo sớm)
        var loaiPhuongTien = LoaiPhuongTien.FromValue(request.LoaiPhuongTienId)!;
        var existingVehicles = await _phuongTienEFRepository.GetPhuongTiensByCanHoIdAsync(request.CanHoId, cancellationToken);
        var currentCount = existingVehicles.Count(x => x.LoaiPhuongTienId == loaiPhuongTien && x.TrangThaiPhuongTienId == TrangThaiPhuongTien.Active);
        var quota = PhuongTienPolicy.GetQuota(canHo.LoaiCanHoId, loaiPhuongTien);

        if (PhuongTienPolicy.IsOverQuota(currentCount, quota))
            return Result.Failure<PhuongTienResponse>(new Error("PhuongTien.QuotaExceeded", $"Căn hộ đã đạt hạn mức tối đa {quota} xe cho loại {loaiPhuongTien.Name}."));

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
            hinhAnhs);

        var phuongTien = await _phuongTienEFRepository.AddAsync(phuongTienEntity, cancellationToken);

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
                NgayBatDau = x.NgayBatDau,
                NgayKetThuc = x.NgayKetThuc,
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
