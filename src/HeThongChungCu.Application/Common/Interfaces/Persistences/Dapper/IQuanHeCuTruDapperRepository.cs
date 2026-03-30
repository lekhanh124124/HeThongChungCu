using HeThongChungCu.Application.Features.CuDan.DTOs;
using HeThongChungCu.Application.Features.CuDan.Queries.LayDSCuTruCuaNguoiDung;
using HeThongChungCu.Application.Features.CuDan.Queries.LayThanhVienCuTru;
using HeThongChungCu.Application.Features.CuDan.Queries.LayThongTinCuDan;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSCuDanTrongChungCu;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface IQuanHeCuTruDapperRepository
{
    Task<PagedResult<CuDanResponse>> LayDSCuDanTrongChungCu(
        LayDSCuDanTrongChungCuQuerySpecification spec,
        CancellationToken cancellationToken = default);


    Task<IReadOnlyList<QuanHeCuTruResponse>> LayDSCuTruByUserId(
        LayDSCuTruCuaNguoiDungSpecification spec,
        CancellationToken cancellationToken = default);

    Task<LayThongTinCuDanResponse?> GetByIdAsync(
        LayThongTinCuDanSpecification spec,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ThanhVienCuTruResponse>> LayThanhVienCuTru(
        LayThanhVienCuTruSpecification spec,
        CancellationToken cancellationToken = default);
}
