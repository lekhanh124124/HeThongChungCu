using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.CapNhatQuanHe;

public record CapNhatQuanHeCommand(
    int QuanHeCuTruId,
    int LoaiQuanHeCuTruId) : ICommand<CuDanResponse>;
