using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLSystem.DTOs;
using HeThongChungCu.Application.Features.QLSystem.Queries.GetBackupHistory;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface ITepTaiLieuQueryRepository
{
    Task<PagedResult<BackupHistoryResponse>> GetBackupHistoryAsync(
        GetBackupHistorySpecification spec,
        CancellationToken cancellationToken = default);
}
