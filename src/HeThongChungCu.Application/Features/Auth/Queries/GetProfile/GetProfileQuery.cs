using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.Auth.DTOs;

namespace HeThongChungCu.Application.Features.Auth.Queries.GetProfile;

public record GetProfileQuery() : IQuery<UserProfileDetailResponse>;
