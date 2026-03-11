using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Domain.Common;
using MediatR;

namespace HeThongChungCu.Application.Features.Profile.Queries.LayQuanHeCuTru;

public record LayQuanHeCuTruQuery : IQuery<IReadOnlyList<LayQuanHeCuTruResponse>>;
