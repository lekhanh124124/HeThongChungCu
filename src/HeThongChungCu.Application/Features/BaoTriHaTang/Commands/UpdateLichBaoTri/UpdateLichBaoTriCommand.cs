using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.UpdateLichBaoTri;

public record UpdateLichBaoTriCommand(
    int Id,
    int TanSuatBaoTriId,
    DateTimeOffset NgayBatDau,
    DateTimeOffset? NgayKetThuc,
    bool IsActive) : ICommand<LichBaoTriResponse>;
