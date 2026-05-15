using System;
using System.Collections.Generic;

namespace HeThongChungCu.Application.Features.QLTaiChinh.DTOs;

public record BaoCaoThuChiResponse
{
    public DateTimeOffset TuNgay { get; init; }
    public DateTimeOffset DenNgay { get; init; }
    public decimal SoDuDauKy { get; init; }
    public decimal TongThu { get; init; }
    public decimal TongChi { get; init; }
    public decimal DongTienThuan { get; init; }
    public decimal SoDuCuoiKy { get; init; }
    public List<BaoCaoThuChiNhomResponse> DanhSachKhoanThu { get; init; } = new();
    public List<BaoCaoThuChiNhomResponse> DanhSachKhoanChi { get; init; } = new();
}
