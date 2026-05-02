using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.ExportChiSoTemplate;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetListChiSo;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IChiSoTieuThuQueryRepository
{
    Task<List<ChiSoExcelTemplateDto>> GetExcelTemplateDataAsync(ExportChiSoTemplateSpecification spec, CancellationToken cancellationToken = default);
    Task<PagedResult<ChiSoResponse>> GetListAsync(GetListChiSoSpecification spec, CancellationToken cancellationToken = default);
    Task<ChiSoDetailResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
