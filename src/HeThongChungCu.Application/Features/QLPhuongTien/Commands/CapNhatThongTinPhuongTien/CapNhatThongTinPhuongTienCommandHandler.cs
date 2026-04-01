using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Application.Features.UploadMedia.DTOs;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.CapNhatThongTinPhuongTien;

internal sealed class CapNhatThongTinPhuongTienCommandHandler : ICommandHandler<CapNhatThongTinPhuongTienCommand, PhuongTienResponse>
{
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaCommandRepository;
    private readonly IPhuongTienCommandRepository _phuongTienCommandRepository;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CapNhatThongTinPhuongTienCommandHandler(
        ICanHoCommandRepository canHoRepository,
        IToaNhaCommandRepository toaNhaCommandRepository,
        IPhuongTienCommandRepository phuongTienCommandRepository,
        ITepTaiLieuRepository tepTaiLieuRepository,
        IUnitOfWork unitOfWork)
    {
        _phuongTienCommandRepository = phuongTienCommandRepository;
        _toaNhaCommandRepository = toaNhaCommandRepository;
        _canHoRepository = canHoRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PhuongTienResponse>> Handle(CapNhatThongTinPhuongTienCommand request, CancellationToken cancellationToken)
    {

        var phuongTien = await _phuongTienCommandRepository.GetPhuongTienByIdAsync(request.PhuongTienId, cancellationToken);
        if (phuongTien == null)
            return Result.Failure<PhuongTienResponse>(PhuongTienErrors.NotFound);

        if (phuongTien.BienSo != request.BienSo)
        {
            var bienSoExists = await _phuongTienCommandRepository.BienSoExistsAsync(request.BienSo, cancellationToken);
            if (bienSoExists)
                return Result.Failure<PhuongTienResponse>(PhuongTienErrors.BienSoExists);
        }

        var canHo = await _canHoRepository.GetByIdAsync(phuongTien.CanHoId, cancellationToken);
        if (canHo == null)
            return Result.Failure<PhuongTienResponse>(CanHoErrors.NotFoundById(phuongTien.CanHoId));

        var toaNha = await _toaNhaCommandRepository.GetToaNhaByTangIdAsync(canHo.TangId, cancellationToken);
        if (toaNha == null)
            return Result.Failure<PhuongTienResponse>(CanHoErrors.NotFoundById(phuongTien.CanHoId));

        var tang = toaNha.Tangs.FirstOrDefault(t => t.Id == canHo.TangId);
        if (tang == null)
            return Result.Failure<PhuongTienResponse>(CanHoErrors.NotFoundById(phuongTien.CanHoId));

        IEnumerable<TepTaiLieu>? hinhAnhs = null;
        if (request.HinhAnhIds != null && request.HinhAnhIds.Count != 0)
        {
            hinhAnhs = await _tepTaiLieuRepository.GetByIdsAsync(request.HinhAnhIds, cancellationToken);
        }

        phuongTien.CapNhat(
            request.TenPhuongTien,
            LoaiPhuongTien.FromValue(request.LoaiPhuongTienId)!,
            request.BienSo,
            request.MauXe,
            hinhAnhs);

        _phuongTienCommandRepository.Update(phuongTien);

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
                TrangThaiThePhuongTienId = x.TrangThaiId.Value,
                TenTrangThaiThePhuongTien = x.TrangThaiId.Name,
            }).ToList(),
            HinhAnhPhuongTiens = phuongTien.HinhAnhPhuongTiens.Select(x => new UploadFileResponse(
                x.Id,
                x.FileName,
                x.FileUrl,
                x.ContentType)).ToList()
        });
    }
}
