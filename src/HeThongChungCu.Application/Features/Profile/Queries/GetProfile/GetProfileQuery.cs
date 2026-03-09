using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.Profile.DTOs;

namespace HeThongChungCu.Application.Features.Profile.Queries.GetProfile;

public record GetProfileQuery() : IQuery<UserProfileDetailResponse>;
