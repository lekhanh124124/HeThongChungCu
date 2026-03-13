namespace HeThongChungCu.Application.Features.QuanHeCuTru.Commands.KetThucCuTru;

public record KetThucCuTruCommand(
    int QuanHeCuTruId) : ICommand<bool>;
