using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLSystem.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLSystem.Queries.GetBackupHistory;

public class GetBackupHistoryQueryHandler : IQueryHandler<GetBackupHistoryQuery, PagedResult<BackupHistoryResponse>>
{
    private readonly ITepTaiLieuQueryRepository _tepQueryRepository;

    public GetBackupHistoryQueryHandler(ITepTaiLieuQueryRepository tepQueryRepository)
    {
        _tepQueryRepository = tepQueryRepository;
    }

    public async Task<Result<PagedResult<BackupHistoryResponse>>> Handle(GetBackupHistoryQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetBackupHistorySpecification(
            request.Keyword,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize);

        var result = await _tepQueryRepository.GetBackupHistoryAsync(spec, cancellationToken);

        return result;
    }
}
