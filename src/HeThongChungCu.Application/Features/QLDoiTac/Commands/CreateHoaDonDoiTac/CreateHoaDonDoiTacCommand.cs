using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.CreateHoaDonDoiTac;

public record CreateHoaDonDoiTacCommand(
    int HopDongDoiTacId,
    int Thang,
    int Nam,
    decimal SoTien,
    int? FileHoaDonId,
    string? GhiChu) : ICommand<HoaDonDoiTacResponse>;
