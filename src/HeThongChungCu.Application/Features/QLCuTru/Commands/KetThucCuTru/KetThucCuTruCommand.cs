using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.KetThucCuTru;

public record KetThucCuTruCommand(
    int QuanHeCuTruId) : ICommand<CuDanResponse>;
