using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.DeletePhuongTien;

public record DeletePhuongTienCommand(List<int> Ids) : ICommand<bool>;
