using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using System.Linq;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.ImportChiSo;

public class ImportChiSoCommandHandler : ICommandHandler<ImportChiSoCommand, ChiSoBatchResultResponse>
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

    public async Task<Result<ChiSoBatchResultResponse>> Handle(ImportChiSoCommand request, CancellationToken cancellationToken)
    {
        var data = _excelService.Import<ChiSoImportDto>(request.FileStream);

        if (data.Count == 0)
        {
            return Result.Failure<ChiSoBatchResultResponse>(new Error("Import.Empty", "File Excel không có dữ liệu hợp lệ."));
        }

        var response = new ChiSoBatchResultResponse
        {
            TotalItems = data.Count
        };

        // Batch load existing records to optimize and avoid duplicates
        var existingChiSos = await _chiSoRepository.GetByPeriodAsync(request.Thang, request.Nam, cancellationToken);
        var existingLookup = existingChiSos
            .ToLookup(x => (x.CanHoId, x.DichVuId));

        var processedInFile = new HashSet<(int CanHoId, int DichVuId)>();
        var newChiSos = new List<ChiSoTieuThu>();

        for (int i = 0; i < data.Count; i++)
        {
            var item = data[i];
            var key = (item.CanHoId, item.DichVuId);

            // Skip if already exists for this period in DB
            if (existingLookup.Contains(key))
            {
                response.Errors.Add(new ChiSoBatchErrorDetail
                {
                    CanHoId = item.CanHoId,
                    Identifier = $"Căn hộ: {item.MaCanHo} - Dịch vụ: {item.TenDichVu}",
                    Reason = "Chỉ số cho kỳ này đã tồn tại trong hệ thống."
                });
                continue;
            }

            // Skip if duplicate within the same Excel file
            if (!processedInFile.Add(key))
            {
                response.Errors.Add(new ChiSoBatchErrorDetail
                {
                    CanHoId = item.CanHoId,
                    Identifier = $"Căn hộ: {item.MaCanHo} - Dịch vụ: {item.TenDichVu}",
                    Reason = "Bị trùng lặp với một dòng khác trong cùng file Excel."
                });
                continue;
            }

            // Check new index must be >= old index
            if (item.SoMoi < item.ChiSoCu)
            {
                response.Errors.Add(new ChiSoBatchErrorDetail
                {
                    CanHoId = item.CanHoId,
                    Identifier = $"Căn hộ: {item.MaCanHo} - Dịch vụ: {item.TenDichVu}",
                    Reason = $"Chỉ số mới ({item.SoMoi}) không được nhỏ hơn chỉ số cũ ({item.ChiSoCu})."
                });
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

        response.SuccessCount = newChiSos.Count;
        response.FailedCount = response.TotalItems - response.SuccessCount;

        return Result.Success(response);
    }
}
