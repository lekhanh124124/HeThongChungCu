using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.ExportChiSoTemplate;

public class ExportChiSoTemplateQueryHandler : IQueryHandler<ExportChiSoTemplateQuery, ExportFileResponse>
{
    private readonly IChiSoTieuThuQueryRepository _queryRepository;
    private readonly IExcelService _excelService;

    public ExportChiSoTemplateQueryHandler(IChiSoTieuThuQueryRepository queryRepository, IExcelService excelService)
    {
        _queryRepository = queryRepository;
        _excelService = excelService;
    }

    public async Task<Result<ExportFileResponse>> Handle(ExportChiSoTemplateQuery request, CancellationToken cancellationToken)
    {
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
