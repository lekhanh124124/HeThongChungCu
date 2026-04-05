using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.XacNhanDinhDanh;

public record XacNhanDinhDanhCommand(string Token) : ICommand<UserInfoResponse>;
