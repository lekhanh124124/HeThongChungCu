using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.AddNhanSuThiCong;

public record AddNhanSuThiCongCommand(
    int Id,
    string HoTen,
    string SoCCCD,
    string? SoDienThoai,
    string? VaiTro,
    string? GhiChu) : ICommand<YeuCauThiCongResponse>;
