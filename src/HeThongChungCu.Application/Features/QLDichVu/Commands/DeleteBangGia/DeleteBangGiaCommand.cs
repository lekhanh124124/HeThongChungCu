using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.DeleteBangGia;

public record DeleteBangGiaCommand(int DichVuId, List<int> Ids) : ICommand<bool>;
