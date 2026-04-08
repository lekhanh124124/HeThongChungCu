using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

namespace HeThongChungCu.Application.Features.QLNhanVien.Commands.DeleteNhanVien;

public record DeleteNhanVienCommand(IReadOnlyList<int> Ids) : ICommand<IReadOnlyList<int>>;
