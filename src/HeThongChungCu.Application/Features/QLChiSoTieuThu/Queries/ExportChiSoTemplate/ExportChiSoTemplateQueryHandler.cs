using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetDichVuById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.ExportChiSoTemplate;

public class ExportChiSoTemplateQueryHandler : IQueryHandler<ExportChiSoTemplateQuery, ExportFileResponse>
{
    private readonly IChiSoTieuThuQueryRepository _queryRepository;
    private readonly IDichVuQueryRepository _dichVuRepository;
    private readonly IExcelService _excelService;

    public ExportChiSoTemplateQueryHandler(
        IChiSoTieuThuQueryRepository queryRepository, 
        IDichVuQueryRepository dichVuRepository,
        IExcelService excelService)
    {
        _queryRepository = queryRepository;
        _dichVuRepository = dichVuRepository;
        _excelService = excelService;
    }

    public async Task<Result<ExportFileResponse>> Handle(ExportChiSoTemplateQuery request, CancellationToken cancellationToken)
    {
        // 1. Validate dịch vụ có tồn tại và đang hoạt động
        var dvSpec = new GetDichVuByIdSpecification(request.DichVuId);
        var service = await _dichVuRepository.GetByIdAsync(dvSpec, cancellationToken);

        if (service == null)
        {
            return Result.Failure<ExportFileResponse>(DichVuErrors.NotFoundById(request.DichVuId));
        }

        if (service.TrangThaiDichVuId != TrangThaiDichVu.HoatDong.Value && 
            service.TrangThaiDichVuId != TrangThaiDichVu.CanhBao.Value)
        {
            return Result.Failure<ExportFileResponse>(DichVuErrors.NotActive(service.TenDichVu));
        }

        // 2 & 3. Validate bảng giá định kỳ và loại Lũy tiến (hoặc các loại cần ghi chỉ số)
        // Lưu ý: Dịch vụ ghi chỉ số tiêu thụ thường dùng bảng giá Lũy tiến (Electricity/Water)
        if (service.BangGia == null || !service.BangGia.IsDinhKy || service.BangGia.LoaiDinhGiaCode != LoaiDinhGia.LuyTien.Code)
        {
            return Result.Failure<ExportFileResponse>(new Error("Export.InvalidServiceType", 
                $"Dịch vụ '{service.TenDichVu}' không phải là dịch vụ tiêu thụ (không có bảng giá lũy tiến định kỳ)."));
        }

        var spec = new ExportChiSoTemplateSpecification(request.DichVuId, request.ToaNhaId, request.TangId, request.Thang, request.Nam);
        var data = await _queryRepository.GetExcelTemplateDataAsync(spec, cancellationToken);
        
        if (data.Count == 0)
        {
            return Result.Failure<ExportFileResponse>(new Error("Export.Empty", "Không tìm thấy căn hộ nào thỏa mãn điều kiện."));
        }

        var bytes = _excelService.CreateTemplate(data, "Chỉ số tiêu thụ");

        return Result.Success(new ExportFileResponse
        {
            Data = bytes,
            FileName = $"Mau_Ghi_Chi_So_{DateTime.Now:yyyyMMddHHmmss}.xlsx",
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        });
    }
}
