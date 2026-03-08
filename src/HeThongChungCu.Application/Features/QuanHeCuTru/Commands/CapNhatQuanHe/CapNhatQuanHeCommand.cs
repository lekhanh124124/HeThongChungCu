namespace HeThongChungCu.Application.Features.QuanHeCuTru.Commands.CapNhatQuanHe;

public record CapNhatQuanHeCommand(
    int QuanHeCuTruId,
    int LoaiQuanHeCuTruId) : ICommand<bool>;
