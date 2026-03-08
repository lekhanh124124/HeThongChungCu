using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;
using HeThongChungCu.Domain.Common;

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

        int totalCount;
        IReadOnlyList<LichSuCuTruResponse> items;

        if (request.CanHoId.HasValue)
        {
            (totalCount, items) = await _queryRepository.GetLichSuByCanHoIdAsync(
                request.CanHoId.Value, request.SortCol, request.IsAsc, request.PageNumber, request.PageSize, cancellationToken);
        }
        else
        {
            (totalCount, items) = await _queryRepository.GetLichSuByUserIdAsync(
                request.UserId!.Value, request.SortCol, request.IsAsc, request.PageNumber, request.PageSize, cancellationToken);
        }

        return Result.Success(new PagedResult<LichSuCuTruResponse>
        {
            Items = items,
            PagingInfo = new PagingInfo
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalCount
            }
        });
    }
}
