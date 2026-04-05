namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal class GetTangByIdReadModel
{
    // Tang
    public int Id { get; set; }
    public string MaTang { get; set; } = string.Empty;
    public string TenTang { get; set; } = string.Empty;
    public int LoaiTangId { get; set; }
    public int ToaNhaId { get; set; }
    public string TenToaNha { get; set; } = string.Empty;

    // CanHo
    public int? CanHoId { get; set; }
    public string? TenTangColumn { get; set; } // Avoid conflict with parent TenTang
    public string? MaCanHo { get; set; }
    public string? TenCanHo { get; set; }
    public decimal? DienTich { get; set; }
    public int? SoPhongNgu { get; set; }
    public int? SoPhongTam { get; set; }
    public int? LoaiCanHoId { get; set; }
    public int? TinhTrangCanHoId { get; set; }
}
