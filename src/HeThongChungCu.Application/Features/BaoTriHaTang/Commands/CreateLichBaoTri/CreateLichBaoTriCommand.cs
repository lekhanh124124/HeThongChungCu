using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CreateLichBaoTri;

public record CreateLichBaoTriCommand(
    int ThietBiId,
    int HangMucBaoTriId,
    int TanSuatBaoTriId,
    DateTimeOffset NgayBatDau,
    DateTimeOffset? NgayKetThuc) : ICommand<LichBaoTriResponse>;
