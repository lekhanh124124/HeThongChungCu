using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.TimHoSoTheoCCCD;

public record TimHoSoTheoCCCDQuery(string IdCard) : IQuery<UserInfoResponse>;
