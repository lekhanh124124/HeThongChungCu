using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.LienKetTaiKhoan;

public record LienKetTaiKhoanCommand(
    int UserId,
    string Email) : ICommand<UserInfoResponse>;
