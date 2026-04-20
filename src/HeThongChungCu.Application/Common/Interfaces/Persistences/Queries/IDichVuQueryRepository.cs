using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IDichVuQueryRepository
{
    Task<PagedResult<DichVuResponse>> GetListAsync(HeThongChungCu.Application.Features.QLDichVu.Queries.GetListDichVu.GetListDichVuSpecification spec, CancellationToken cancellationToken = default);
    Task<DichVuDetailResponse?> GetByIdAsync(HeThongChungCu.Application.Features.QLDichVu.Queries.GetDichVuById.GetDichVuByIdSpecification spec, CancellationToken cancellationToken = default);
    Task<PagedResult<KhungGioDichVuResponse>> GetListKhungGioAsync(HeThongChungCu.Application.Features.QLDichVu.Queries.GetListKhungGioDichVu.GetListKhungGioDichVuSpecification spec, CancellationToken cancellationToken = default);
    Task<KhungGioDichVuResponse?> GetKhungGioByIdAsync(HeThongChungCu.Application.Features.QLDichVu.Queries.GetKhungGioDichVuById.GetKhungGioDichVuByIdSpecification spec, CancellationToken cancellationToken = default);
    Task<PagedResult<BangGiaResponse>> GetListBangGiaAsync(HeThongChungCu.Application.Features.QLDichVu.Queries.GetListBangGia.GetListBangGiaSpecification spec, CancellationToken cancellationToken = default);
    Task<BangGiaResponse?> GetBangGiaByIdAsync(HeThongChungCu.Application.Features.QLDichVu.Queries.GetBangGiaById.GetBangGiaByIdSpecification spec, CancellationToken cancellationToken = default);
    Task<PagedResult<DangKyDichVuResponse>> GetListDangKyAsync(HeThongChungCu.Application.Features.QLDichVu.Queries.GetListDangKyDichVu.GetListDangKyDichVuSpecification spec, CancellationToken cancellationToken = default);
}
