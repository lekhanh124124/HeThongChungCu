using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.CapNhatThongTinPhuongTien;

internal sealed class CapNhatThongTinPhuongTienCommandHandler : ICommandHandler<CapNhatThongTinPhuongTienCommand, PhuongTienResponse>
{
    private readonly ICanHoEFRepository _canHoRepository;
    private readonly IToaNhaEFRepository _toaNhaEFRepository;
    private readonly IPhuongTienEFRepository _phuongTienEFRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CapNhatThongTinPhuongTienCommandHandler(
        ICanHoEFRepository canHoRepository,
        IToaNhaEFRepository toaNhaEFRepository,
        IPhuongTienEFRepository phuongTienEFRepository,
        IUnitOfWork unitOfWork)
    {
        _phuongTienEFRepository = phuongTienEFRepository;
        _toaNhaEFRepository = toaNhaEFRepository;
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
        if (canHo == null)
            return Result.Failure<PhuongTienResponse>(CanHoErrors.NotFoundById(phuongTien.CanHoId));

        var toaNha = await _toaNhaEFRepository.GetToaNhaByTangIdAsync(canHo.TangId, cancellationToken);
        if (toaNha == null)
            return Result.Failure<PhuongTienResponse>(CanHoErrors.NotFoundById(phuongTien.CanHoId));

        var tang = toaNha.Tangs.FirstOrDefault(t => t.Id == canHo.TangId);
        if (tang == null)
            return Result.Failure<PhuongTienResponse>(CanHoErrors.NotFoundById(phuongTien.CanHoId));

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
            MaToaNha = toaNha.MaToaNha,
            MaTang = tang.MaTang,
            MaCanHo = canHo.MaCanHo,
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
                IsLocked = x.IsLocked,
            }).ToList()
        });
    }
}
