using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.NhanVien.DTOs;
using HeThongChungCu.Application.Features.NhanVien.Queries.GetNhanVienById;
using HeThongChungCu.Application.Features.NhanVien.Queries.GetNhanVienList;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface INhanVienQueryRepository
{
    Task<NhanVienResponse?> GetByIdAsync(GetNhanVienByIdSpecification spec, CancellationToken cancellationToken = default);
    Task<PagedResult<NhanVienResponse>> GetListAsync(GetNhanVienListSpecification spec, CancellationToken cancellationToken = default);
}
