using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.UpdateChiSoTieuThu;

public record UpdateChiSoTieuThuCommand(
    int Id,
    decimal ChiSoCu,
    decimal ChiSoMoi,
    int Thang,
    int Nam,
    DateTimeOffset NgayGhiNhan,
    int? AnhDongHoId,
    string? GhiChu) : ICommand<ChiSoDetailResponse>;
