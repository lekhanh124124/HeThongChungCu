using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;
using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetYeuCauThiCongById;

public record GetYeuCauThiCongByIdQuery(int Id) : IQuery<YeuCauThiCongDetailResponse?>;
