using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetListChiSo;

public class GetListChiSoQueryHandler : IQueryHandler<GetListChiSoQuery, PagedResult<ChiSoResponse>>
{
    private readonly IChiSoTieuThuQueryRepository _repository;

    public GetListChiSoQueryHandler(IChiSoTieuThuQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PagedResult<ChiSoResponse>>> Handle(GetListChiSoQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetListChiSoSpecification(
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize,
            request.Thang,
            request.Nam,
            request.DichVuId,
            request.TrangThaiChiSoId);

        var result = await _repository.GetListAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
