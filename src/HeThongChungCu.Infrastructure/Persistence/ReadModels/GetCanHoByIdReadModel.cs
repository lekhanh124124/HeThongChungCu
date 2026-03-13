namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

public class GetCanHoByIdReadModel
{
    // CanHo
    public int Id { get; set; }
    public int TangId { get; set; }
    public string TenTang { get; set; } = string.Empty;
    public string MaCanHo { get; set; } = string.Empty;
    public decimal DienTich { get; set; }
    public int SoPhongNgu { get; set; }
    public int SoPhongTam { get; set; }
    public int LoaiCanHoId { get; set; }
    public int TinhTrangCanHoId { get; set; }

    // QuanHeCuTru
    public int? QuanHeCuTruId { get; set; }
    public int? UserId { get; set; }
    public string? FullName { get; set; }
    public int? LoaiQuanHeCuTruId { get; set; }
    public DateTime? NgayBatDau { get; set; }
    public DateTime? NgayKetThuc { get; set; }
    public bool? IsKetThuc { get; set; }
}
