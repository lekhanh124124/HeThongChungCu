using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using System.Linq;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.ImportChiSo;

public class ImportChiSoCommandHandler : ICommandHandler<ImportChiSoCommand, int>
{
    private readonly IChiSoTieuThuCommandRepository _chiSoRepository;
    private readonly IExcelService _excelService;
    private readonly IUnitOfWork _unitOfWork;

    public ImportChiSoCommandHandler(
        IChiSoTieuThuCommandRepository chiSoRepository, 
        IExcelService excelService, 
        IUnitOfWork unitOfWork)
    {
        _chiSoRepository = chiSoRepository;
        _excelService = excelService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(ImportChiSoCommand request, CancellationToken cancellationToken)
    {
        var data = _excelService.Import<ChiSoImportDto>(request.FileStream);

        if (data.Count == 0)
        {
            return Result.Failure<int>(new Error("Import.Empty", "File Excel không có dữ liệu hợp lệ."));
        }

        // Batch load existing records to optimize and avoid duplicates
        var existingChiSos = await _chiSoRepository.GetByPeriodAsync(request.Thang, request.Nam, cancellationToken);
        var existingLookup = existingChiSos
            .ToLookup(x => (x.CanHoId, x.DichVuId));

        var newChiSos = new List<ChiSoTieuThu>();

        foreach (var item in data)
        {
            // Skip if already exists for this period
            if (existingLookup.Contains((item.CanHoId, item.DichVuId)))
            {
                continue;
            }

            // Check new index must be >= old index
            if (item.SoMoi < item.ChiSoCu)
            {
                continue; 
            }

            var chiSo = ChiSoTieuThu.Create(
                item.CanHoId,
                item.DichVuId,
                item.ChiSoCu,
                item.SoMoi,
                request.Thang,
                request.Nam,
                request.NgayGhiNhan,
                null, // AnhDongHoId
                item.GhiChu,
                item.MaTraCuu
            );

            newChiSos.Add(chiSo);
        }

        if (newChiSos.Count > 0)
        {
            await _chiSoRepository.AddRangeAsync(newChiSos, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success(newChiSos.Count);
    }
}
