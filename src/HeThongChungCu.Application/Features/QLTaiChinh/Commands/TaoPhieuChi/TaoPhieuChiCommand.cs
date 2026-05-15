using HeThongChungCu.Application.Common.Messaging;
using System;
using System.Collections.Generic;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Commands.TaoPhieuChi;

public record TaoPhieuChiCommand : ICommand<int>
{
    public DateTimeOffset NgayGiaoDich { get; init; } = DateTimeOffset.UtcNow;
    public int PhuongThucThanhToanId { get; init; }
    public string NguoiGiaoDich { get; init; } = null!;
    public string? ChungTuGoc { get; init; }
    public List<ChiTietChiDto> ChiTiets { get; init; } = new();
}

public record ChiTietChiDto
{
    public string NhomThongKe { get; init; } = null!;
    public decimal SoTien { get; init; }
    public string? GhiChu { get; init; }
}
