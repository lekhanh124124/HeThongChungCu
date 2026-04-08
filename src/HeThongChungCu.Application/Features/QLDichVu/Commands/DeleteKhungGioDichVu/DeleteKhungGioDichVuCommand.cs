using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.DeleteKhungGioDichVu;

public record DeleteKhungGioDichVuCommand(int DichVuId, List<int> Ids) : ICommand<bool>;
