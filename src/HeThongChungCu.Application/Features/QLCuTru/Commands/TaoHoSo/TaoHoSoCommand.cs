using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoHoSo;

public record TaoHoSoCommand(
    string FirstName,
    string LastName,
    DateTime Dob,
    int GioiTinhId,
    string? DiaChi,
    string? IdCard,
    string? PhoneNumber,
    List<TaiLieuRequest>? TaiLieuCuTrus = null) : ICommand<UserInfoResponse>;