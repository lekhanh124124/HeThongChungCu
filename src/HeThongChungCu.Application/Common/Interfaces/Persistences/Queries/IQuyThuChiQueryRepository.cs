using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLTaiChinh.DTOs;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetNhatKyThuChi;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoThuChi;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoCongNoCanHo;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoCongNoToaNha;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetQuyThuChiById;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IQuyThuChiQueryRepository
{
    Task<PagedResult<QuyThuChiResponse>> GetNhatKyThuChiAsync(
        GetNhatKyThuChiSpecification spec,
        CancellationToken cancellationToken = default);

    Task<QuyThuChiResponse?> GetByIdAsync(GetQuyThuChiByIdSpecification spec, CancellationToken cancellationToken = default);

    Task<BaoCaoThuChiResponse> GetBaoCaoThuChiAsync(
        GetBaoCaoThuChiSpecification spec,
        CancellationToken cancellationToken = default);

    Task<List<BaoCaoCongNoCanHoResponse>> GetBaoCaoCongNoCanHoAsync(
        GetBaoCaoCongNoCanHoSpecification spec,
        CancellationToken cancellationToken = default);

    Task<List<BaoCaoCongNoToaNhaResponse>> GetBaoCaoCongNoToaNhaAsync(
        GetBaoCaoCongNoToaNhaSpecification spec,
        CancellationToken cancellationToken = default);
}
