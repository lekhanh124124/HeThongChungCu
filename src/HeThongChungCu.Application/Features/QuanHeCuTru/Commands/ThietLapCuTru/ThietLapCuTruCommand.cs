using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Commands.ThietLapCuTru;

public record ThietLapCuTruCommand(
    int CanHoId,
    int UserId,
    int LoaiQuanHeCuTruId) : ICommand<CuDanResponse>;
