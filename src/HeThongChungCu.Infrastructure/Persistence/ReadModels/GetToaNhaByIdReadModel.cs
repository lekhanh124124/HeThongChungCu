namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal class GetToaNhaByIdReadModel
{
    // ToaNha
    public int Id { get; set; }
    public string MaToaNha { get; set; } = string.Empty;
    public string TenToaNha { get; set; } = string.Empty;
    public string? DiaChi { get; set; }
    public string? MoTa { get; set; }
    public int TrangThaiToaNhaId { get; set; }
    public int SoCanHo { get; set; }

    // CanHo
    public int? CanHoId { get; set; }
    public string? MaCanHo { get; set; }
    public int? TangId { get; set; }
    public string? TenTang { get; set; }
    public decimal? DienTich { get; set; }
    public int? SoPhongNgu { get; set; }
    public int? SoPhongTam { get; set; }
    public int? LoaiCanHoId { get; set; }
    public int? TinhTrangCanHoId { get; set; }
}
