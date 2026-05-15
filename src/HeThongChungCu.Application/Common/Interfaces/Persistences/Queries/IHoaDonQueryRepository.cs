using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetHoaDonById;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetListHoaDon;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IHoaDonQueryRepository
{
    Task<PagedResult<HoaDonResponse>> GetListAsync(
        GetListHoaDonSpecification spec,
        CancellationToken cancellationToken = default);

    Task<HoaDonDetailResponse?> GetByIdAsync(
        GetHoaDonByIdSpecification spec,
        CancellationToken cancellationToken = default);

    Task<ChiTietCoDinhResponse?> GetChiTietCoDinhAsync(int chiTietHoaDonId, CancellationToken cancellationToken = default);
    Task<ChiTietLuyTienResponse?> GetChiTietLuyTienAsync(int chiTietHoaDonId, CancellationToken cancellationToken = default);
    Task<ChiTietDienTichResponse?> GetChiTietDienTichAsync(int chiTietHoaDonId, CancellationToken cancellationToken = default);
    Task<ChiTietKhungGioResponse?> GetChiTietKhungGioAsync(int chiTietHoaDonId, CancellationToken cancellationToken = default);
    Task<(string TenMucPhi, int LoaiChiTietHoaDonId, string? ResidentName, int? DichVuId)> GetChiTietHoaDonInfoAsync(int chiTietHoaDonId, CancellationToken cancellationToken = default);
}
