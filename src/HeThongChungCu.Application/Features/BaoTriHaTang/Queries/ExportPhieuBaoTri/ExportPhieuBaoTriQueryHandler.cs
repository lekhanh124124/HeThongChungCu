using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetPhieuBaoTriById;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.ExportPhieuBaoTri;

public class ExportPhieuBaoTriQueryHandler : IQueryHandler<ExportPhieuBaoTriQuery, ExportExcelResponse>
{
    private readonly IPhieuBaoTriQueryRepository _queryRepository;
    private readonly IExcelService _excelService;

    public ExportPhieuBaoTriQueryHandler(IPhieuBaoTriQueryRepository queryRepository, IExcelService excelService)
    {
        _queryRepository = queryRepository;
        _excelService = excelService;
    }

    public async Task<Result<ExportExcelResponse>> Handle(ExportPhieuBaoTriQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetPhieuBaoTriByIdSpecification(request.Id);
        var phieu = await _queryRepository.GetByIdAsync(spec, cancellationToken);
        if (phieu == null)
            return BaoTriHaTangErrors.PhieuBaoTriNotFoundById(request.Id);

        var fileBytes = _excelService.ExportPhieuBaoTri(phieu);
        var fileName = $"Phieu_Bao_Tri_{phieu.MaPhieu}.xlsx";
        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        return Result.Success(new ExportExcelResponse(fileBytes, contentType, fileName));
    }
}
