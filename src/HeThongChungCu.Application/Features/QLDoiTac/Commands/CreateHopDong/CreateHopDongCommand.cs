using HeThongChungCu.Application.Features.QLDoiTac.DTOs;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.CreateHopDong;

public record CreateHopDongCommand(
    int DoiTacId,
    HopDongRequestDto HopDong) : ICommand<DoiTacDetailResponse>;
