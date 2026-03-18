using HeThongChungCu.Application.Features.PhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.PhuongTien.Commands.CapNhatThongTinPhuongTien;

internal sealed class CapNhatThongTinPhuongTienCommandHandler : ICommandHandler<CapNhatThongTinPhuongTienCommand, PhuongTienResponse>
{
    private readonly ICanHoEFRepository _canHoRepository;
    private readonly IPhuongTienEFRepository _phuongTienEFRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CapNhatThongTinPhuongTienCommandHandler(
        ICanHoEFRepository canHoRepository,
        IPhuongTienEFRepository phuongTienEFRepository,
        IUnitOfWork unitOfWork)
    {
        _phuongTienEFRepository = phuongTienEFRepository;
        _canHoRepository = canHoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PhuongTienResponse>> Handle(CapNhatThongTinPhuongTienCommand request, CancellationToken cancellationToken)
    {
        
        var phuongTien = await _phuongTienEFRepository.GetPhuongTienByIdAsync(request.PhuongTienId, cancellationToken);
        if (phuongTien == null)
            return Result.Failure<PhuongTienResponse>(PhuongTienErrors.NotFound);

        if (phuongTien.BienSo != request.BienSo)
        {
            var bienSoExists = await _phuongTienEFRepository.BienSoExistsAsync(request.BienSo, cancellationToken);
            if (bienSoExists)
                return Result.Failure<PhuongTienResponse>(PhuongTienErrors.BienSoExists);
        }

        var canHo = await _canHoRepository.GetByIdAsync(phuongTien.CanHoId, cancellationToken);

        phuongTien.UpdateInfo(
            request.TenPhuongTien,
            LoaiPhuongTien.FromValue(request.LoaiPhuongTienId)!,
            request.BienSo,
            request.MauXe);

        _phuongTienEFRepository.Update(phuongTien);

        // TransactionBehavior will automatically commit if no exception is thrown, otherwise it will rollback

        return Result.Success(new PhuongTienResponse
        {
            Id = phuongTien.Id,
            MaToaNha = canHo?.Tang.ToaNha.MaToaNha ?? string.Empty,
            MaTang = canHo?.Tang.MaTang ?? string.Empty,
            MaCanHo = canHo?.MaCanHo ?? string.Empty,
            BienSo = phuongTien.BienSo,
            LoaiPhuongTienId = phuongTien.LoaiPhuongTienId.Value,
            TenLoaiPhuongTien = phuongTien.LoaiPhuongTienId.Name,
            MauXe = phuongTien.MauXe,
            TenPhuongTien = phuongTien.TenPhuongTien,
            TrangThaiPhuongTienId = phuongTien.TrangThaiPhuongTienId.Value,
            TenTrangThaiPhuongTien = phuongTien.TrangThaiPhuongTienId.Name,
            ThePhuongTiens = phuongTien.ThePhuongTiens.Select(x => new ThePhuongTienResponse
            {
                Id = x.Id,
                PhuongTienId = x.PhuongTienId,
                MaThe = x.MaThe,
                NgayBatDau = x.NgayBatDau,
                NgayKetThuc = x.NgayKetThuc,
                IsActive = x.IsLocked,
            }).ToList()
        });
    }
}
