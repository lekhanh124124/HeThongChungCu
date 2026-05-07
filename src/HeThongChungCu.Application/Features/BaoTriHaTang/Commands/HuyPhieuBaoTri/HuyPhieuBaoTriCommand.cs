namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.HuyPhieuBaoTri;

public record HuyPhieuBaoTriCommand(int Id, string LyDo) : ICommand<bool>;
