using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.ThietLapCuTru;

public record ThietLapCuTruCommand(
    int CanHoId,
    int UserId,
    int LoaiQuanHeCuTruId) : ICommand<CuDanResponse>;
