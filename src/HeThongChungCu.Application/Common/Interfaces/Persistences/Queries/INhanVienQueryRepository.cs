using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLNhanVien.DTOs;
using HeThongChungCu.Application.Features.QLNhanVien.Queries.GetNhanVienById;
using HeThongChungCu.Application.Features.QLNhanVien.Queries.GetNhanVienList;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface INhanVienQueryRepository
{
    Task<NhanVienDetailResponse?> GetByIdAsync(GetNhanVienByIdSpecification spec, CancellationToken cancellationToken = default);
    Task<PagedResult<NhanVienResponse>> GetListAsync(GetNhanVienListSpecification spec, CancellationToken cancellationToken = default);
}
