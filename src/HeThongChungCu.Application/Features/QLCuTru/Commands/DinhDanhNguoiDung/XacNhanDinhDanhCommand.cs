using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.DinhDanhNguoiDung;

public record XacNhanDinhDanhCommand(string Token) : ICommand<UserInfoResponse>;
