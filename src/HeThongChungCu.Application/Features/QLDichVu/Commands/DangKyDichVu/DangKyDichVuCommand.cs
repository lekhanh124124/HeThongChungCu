using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.DangKyDichVu;

public record DangKyDichVuCommand(
    int CanHoId,
    int DichVuId,
    DateTimeOffset NgaySuDung,
    int SoLuong = 1,
    int? KhungGioId = null) : ICommand<int>;
