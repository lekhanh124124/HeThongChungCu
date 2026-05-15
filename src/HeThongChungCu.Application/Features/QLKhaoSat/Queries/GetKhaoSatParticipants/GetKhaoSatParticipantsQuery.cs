using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatParticipants;

public class GetKhaoSatParticipantsQuery : IQuery<PagedResult<KhaoSatParticipantResponse>>
{
    public int KhaoSatId { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}
