using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.NhapBaoGiaYeuCauSuaChua;

public record NhapBaoGiaYeuCauSuaChuaCommand(
    int Id,
    decimal ChiPhiDuKien,
    bool IsMienPhi,
    string? GhiChuBaoGia) : ICommand<YeuCauSuaChuaDetailResponse>;
