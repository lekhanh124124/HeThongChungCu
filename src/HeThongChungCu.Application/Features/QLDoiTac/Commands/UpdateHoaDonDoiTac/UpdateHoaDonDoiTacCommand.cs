using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.UpdateHoaDonDoiTac;

public record UpdateHoaDonDoiTacCommand(
    int Id,
    int Thang,
    int Nam,
    decimal SoTien,
    int? FileHoaDonId,
    string? GhiChu) : ICommand<HoaDonDoiTacResponse>;
