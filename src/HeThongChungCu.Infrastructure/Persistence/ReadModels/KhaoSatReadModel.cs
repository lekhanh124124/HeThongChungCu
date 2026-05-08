using System;

namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

public class KhaoSatReadModel
{
    public int Id { get; set; }
    public string TieuDe { get; set; } = string.Empty;
    public string MoTa { get; set; } = string.Empty;
    public int LoaiKhaoSatId { get; set; }
    public int CoCheTinhDiemId { get; set; }
    public int TrangThaiId { get; set; }
    public DateTimeOffset NgayBatDau { get; set; }
    public DateTimeOffset NgayKetThuc { get; set; }
    public decimal TyleThamGiaToiThieu { get; set; }
    public decimal TyLeDongYToiThieu { get; set; }
    public bool IsAnDanh { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public int TotalCount { get; set; }
}
