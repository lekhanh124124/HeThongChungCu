namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.DeleteHangMucBaoTri;

public record DeleteHangMucBaoTriCommand(int Id) : ICommand<bool>;
