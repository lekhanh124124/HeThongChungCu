using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.NhanVien.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.NhanVien.Queries.GetNhanVienList;

public class GetNhanVienListQueryHandler : IQueryHandler<GetNhanVienListQuery, PagedResult<NhanVienResponse>>
{
    private readonly INhanVienQueryRepository _nhanVienQueryRepository;

    public GetNhanVienListQueryHandler(INhanVienQueryRepository nhanVienQueryRepository)
    {
        _nhanVienQueryRepository = nhanVienQueryRepository;
    }

    public async Task<Result<PagedResult<NhanVienResponse>>> Handle(GetNhanVienListQuery request, CancellationToken cancellationToken)
    {
        var result = await _nhanVienQueryRepository.GetListAsync(new GetNhanVienListSpecification(
            request.Keyword,
            request.LoaiNhanVienId,
            request.TrangThaiNhanVienId,
            request.PageNumber,
            request.PageSize), cancellationToken);

        return Result.Success(result);
    }
}
