using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.KiemDuyetPhieuBaoTri;

public record KiemDuyetPhieuBaoTriCommand(
    int Id,
    bool IsDuyet,
    string? GhiChuXuLy) : ICommand<PhieuBaoTriDetailResponse>;
