using System;

namespace HeThongChungCu.Application.Features.QLKhaoSat.DTOs;

public class KhaoSatResponse
{
    public int Id { get; set; }
    public string TieuDe { get; set; } = string.Empty;
    public string MoTa { get; set; } = string.Empty;
    public int LoaiKhaoSatId { get; set; }
    public string LoaiKhaoSatTen { get; set; } = string.Empty;
    public int CoCheTinhDiemId { get; set; }
    public string CoCheTinhDiemTen { get; set; } = string.Empty;
    public int TrangThaiId { get; set; }
    public string TrangThaiTen { get; set; } = string.Empty;
    public DateTimeOffset NgayBatDau { get; set; }
    public DateTimeOffset NgayKetThuc { get; set; }
    public decimal TyleThamGiaToiThieu { get; set; }
    public decimal TyLeDongYToiThieu { get; set; }
    public bool IsAnDanh { get; set; }
    public bool IsVoted { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
