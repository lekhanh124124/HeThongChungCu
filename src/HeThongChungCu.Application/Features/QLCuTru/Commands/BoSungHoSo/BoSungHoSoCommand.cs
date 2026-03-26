using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.BoSungHoSo;

public record BoSungHoSoCommand(
    int UserId,
    List<BoSungHoSoItemCommand> Documents) : ICommand<UserInfoResponse>;

public record BoSungHoSoItemCommand(
    int LoaiGiayToId,
    string SoGiayTo,
    DateTime? NgayPhatHanh,
    List<int> FileIds);
