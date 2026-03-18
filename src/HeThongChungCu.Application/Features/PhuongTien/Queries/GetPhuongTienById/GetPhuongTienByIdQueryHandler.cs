using HeThongChungCu.Application.Features.PhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.PhuongTien.Queries.GetPhuongTienById;

public class GetPhuongTienByIdQueryHandler : IQueryHandler<GetPhuongTienByIdQuery, PhuongTienResponse>
{
    private readonly IPhuongTienEFRepository _phuongTienEFRepository;
    private readonly ICanHoEFRepository _canHoRepository;

    public GetPhuongTienByIdQueryHandler(
        IPhuongTienEFRepository phuongTienEFRepository,
        ICanHoEFRepository canHoRepository)
    {
        _phuongTienEFRepository = phuongTienEFRepository;
        _canHoRepository = canHoRepository;
    }

    public async Task<Result<PhuongTienResponse>> Handle(GetPhuongTienByIdQuery request, CancellationToken cancellationToken)
    {
        var phuongTien = await _phuongTienEFRepository.GetPhuongTienByIdAsync(request.Id, cancellationToken);
        if (phuongTien == null)
            return Result.Failure<PhuongTienResponse>(PhuongTienErrors.NotFound);

        var canHo = await _canHoRepository.GetByIdAsync(phuongTien.CanHoId, cancellationToken);

        var result = new PhuongTienResponse
        {
            Id = phuongTien.Id,
            MaToaNha = canHo?.Tang.ToaNha.MaToaNha ?? string.Empty,
            MaTang = canHo?.Tang.MaTang ?? string.Empty,
            MaCanHo = canHo?.MaCanHo ?? string.Empty,
            TenPhuongTien = phuongTien.TenPhuongTien,
            LoaiPhuongTienId = phuongTien.LoaiPhuongTienId.Value,
            TenLoaiPhuongTien = phuongTien.LoaiPhuongTienId.Name,
            BienSo = phuongTien.BienSo,
            MauXe = phuongTien.MauXe,
            TrangThaiPhuongTienId = phuongTien.TrangThaiPhuongTienId.Value,
            TenTrangThaiPhuongTien = phuongTien.TrangThaiPhuongTienId.Name,
            ThePhuongTiens = phuongTien.ThePhuongTiens.Select(t => new ThePhuongTienResponse
            {
                Id = t.Id,
                PhuongTienId = t.PhuongTienId,
                MaThe = t.MaThe,
                NgayBatDau = t.NgayBatDau,
                NgayKetThuc = t.NgayKetThuc,
                IsActive = t.IsLocked
            }).ToList()
        };

        return Result.Success(result);
    }
}
