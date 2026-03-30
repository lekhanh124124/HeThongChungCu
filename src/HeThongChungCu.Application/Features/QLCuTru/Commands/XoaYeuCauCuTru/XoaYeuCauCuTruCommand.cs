namespace HeThongChungCu.Application.Features.QLCuTru.Commands.XoaYeuCauCuTru;

public record XoaYeuCauCuTruCommand(List<int> Ids) : ICommand<bool>;
