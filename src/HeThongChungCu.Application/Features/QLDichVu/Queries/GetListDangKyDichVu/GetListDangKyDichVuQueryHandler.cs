using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;


namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetListDangKyDichVu;

public class GetListDangKyDichVuQueryHandler : IQueryHandler<GetListDangKyDichVuQuery, PagedResult<DangKyDichVuResponse>>
{
    private readonly IDichVuQueryRepository _dichVuQueryRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetListDangKyDichVuQueryHandler(
        IDichVuQueryRepository dichVuQueryRepository,
        ICurrentUserService currentUserService)
    {
        _dichVuQueryRepository = dichVuQueryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PagedResult<DangKyDichVuResponse>>> Handle(GetListDangKyDichVuQuery request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId == null)
            return UserErrors.NotFound;

        var userId = _currentUserService.UserId.Value;

        var spec = new GetListDangKyDichVuSpecification(
            userId,
            request.LoaiDichVuId,
            request.DichVuId,
            request.TrangThaiDangKyId,
            request.TuNgay,
            request.DenNgay,
            request.Keyword,
            request.PageNumber,
            request.PageSize,
            request.SortCol,
            request.IsAsc);

        var result = await _dichVuQueryRepository.GetListDangKyAsync(spec, cancellationToken);
        return result;
    }
}
