using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayLichSuCuTru;

public class LayLichSuCuTruQueryHandler : IQueryHandler<LayLichSuCuTruQuery, PagedResult<LichSuCuTruResponse>>
{
    private readonly IQuanHeCuTruDapperRepository _queryRepository;

    public LayLichSuCuTruQueryHandler(IQuanHeCuTruDapperRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<LichSuCuTruResponse>>> Handle(LayLichSuCuTruQuery request, CancellationToken cancellationToken)
    {
        if (request.CanHoId is null && request.UserId is null)
            return Result.Failure<PagedResult<LichSuCuTruResponse>>(new Error(
                "QuanHeCuTru.InvalidFilter",
                "Phải cung cấp ít nhất CanHoId hoặc UserId để lấy lịch sử cư trú."));

        var spec = new LayLichSuCuTruSpecification(
            request.CanHoId,
            request.UserId,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize);

        var result = await _queryRepository.GetLichSuAsync(spec, cancellationToken);

        return Result.Success(result);
    }
}
