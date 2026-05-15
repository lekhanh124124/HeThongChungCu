using HeThongChungCu.Application.Common.Messaging;
using System;
using System.Collections.Generic;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Commands.TaoPhieuThu;

public record TaoPhieuThuCommand : ICommand<int>
{
    public DateTimeOffset NgayGiaoDich { get; init; } = DateTimeOffset.UtcNow;
    public int PhuongThucThanhToanId { get; init; }
    public string NguoiGiaoDich { get; init; } = null!;
    public string? ChungTuGoc { get; init; }
    public List<ChiTietThuDto> ChiTiets { get; init; } = new();
}

public record ChiTietThuDto
{
    public int? DichVuId { get; init; }
    public decimal SoTien { get; init; }
    public string NhomThongKe { get; init; } = null!;
    public string? GhiChu { get; init; }
}
