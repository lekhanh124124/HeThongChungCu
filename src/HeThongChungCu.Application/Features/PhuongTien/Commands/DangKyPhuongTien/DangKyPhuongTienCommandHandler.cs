using HeThongChungCu.Application.Features.PhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.PhuongTien.Commands.DangKyPhuongTien;

internal sealed class DangKyPhuongTienCommandHandler : ICommandHandler<DangKyPhuongTienCommand, PhuongTienResponse>
{
    private readonly IPhuongTienEFRepository _phuongTienEFRepository;
    private readonly ICanHoEFRepository _canHoEFRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DangKyPhuongTienCommandHandler(
        IPhuongTienEFRepository phuongTienEFRepository,
        ICanHoEFRepository canHoEFRepository,
        IUnitOfWork unitOfWork)
    {
        _phuongTienEFRepository = phuongTienEFRepository;
        _canHoEFRepository = canHoEFRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PhuongTienResponse>> Handle(DangKyPhuongTienCommand request, CancellationToken cancellationToken)
    {
        var canHo = await _canHoEFRepository.GetByIdAsync(request.CanHoId, cancellationToken);
        if (canHo == null)
            return Result.Failure<PhuongTienResponse>(CanHoErrors.NotFoundById(request.CanHoId));

        var bienSoExists = await _phuongTienEFRepository.BienSoExistsAsync(request.BienSo, cancellationToken);
        if (bienSoExists)
            return Result.Failure<PhuongTienResponse>(PhuongTienErrors.BienSoExists);

        var phuongTien = new Domain.Entities.PhuongTien(
            request.CanHoId,
            request.TenPhuongTien,
            LoaiPhuongTien.FromValue(request.LoaiPhuongTienId)!,
            request.BienSo,
            request.MauXe);

        await _phuongTienEFRepository.AddAsync(phuongTien, cancellationToken);

        // TransactionBehavior will automatically commit if no exception is thrown, otherwise it will rollback

        return Result.Success(new PhuongTienResponse
        {
            Id = phuongTien.Id,
            MaToaNha = canHo.Tang.ToaNha.MaToaNha,
            MaTang = canHo.Tang.MaTang,
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
                IsActive = x.IsLocked,
             }).ToList()
        });
    }
}
