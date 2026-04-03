using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

namespace HeThongChungCu.Application.Features.NhanVien.Commands.DeleteNhanVien;

public record DeleteNhanVienCommand(IReadOnlyList<int> Ids) : ICommand<IReadOnlyList<int>>;
