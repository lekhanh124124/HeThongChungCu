using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.ActivateBangGia;

public record ActivateBangGiaCommand(int DichVuId, List<int> Ids) : ICommand<bool>;
