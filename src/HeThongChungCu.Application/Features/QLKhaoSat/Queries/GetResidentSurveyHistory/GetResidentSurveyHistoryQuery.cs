using System.Collections.Generic;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetResidentSurveyHistory;

public class GetResidentSurveyHistoryQuery : IQuery<List<ResidentSurveyHistoryResponse>>
{
    public int? CanHoId { get; set; }
    public int? KhaoSatId { get; set; }
}
