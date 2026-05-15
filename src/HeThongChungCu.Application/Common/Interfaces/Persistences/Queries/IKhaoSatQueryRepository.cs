using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatList;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatById;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKetQuaKhaoSat;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatParticipants;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetResidentSurveyHistory;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IKhaoSatQueryRepository
{
    Task<PagedResult<KhaoSatResponse>> GetAllAsync(GetKhaoSatListSpecification spec, CancellationToken cancellationToken = default);
    Task<KhaoSatDetailResponse?> GetByIdAsync(GetKhaoSatByIdSpecification spec, CancellationToken cancellationToken = default);
    Task<KetQuaKhaoSatResponse?> GetKetQuaKhaoSatAsync(GetKetQuaKhaoSatSpecification spec, CancellationToken cancellationToken = default);
    Task<PagedResult<KhaoSatParticipantResponse>> GetParticipantsAsync(GetKhaoSatParticipantsSpecification spec, CancellationToken cancellationToken = default);
    Task<List<ResidentSurveyHistoryResponse>> GetResidentHistoryAsync(GetResidentSurveyHistorySpecification spec, CancellationToken cancellationToken = default);
}
