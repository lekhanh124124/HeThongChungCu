using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.CanHo.Queries.GoiYMaCanHo;

public record GoiYMaCanHoQuery(int TangId) : IQuery<string>;
