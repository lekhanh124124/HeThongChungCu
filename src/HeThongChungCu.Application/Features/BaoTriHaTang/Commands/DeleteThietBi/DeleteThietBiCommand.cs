namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.DeleteThietBi;

public record DeleteThietBiCommand(int Id) : ICommand<bool>;
