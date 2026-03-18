namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal class GetCanHoByIdReadModel
{
    // CanHo
    public int Id { get; set; }
    public int TangId { get; set; }
    public string TenTang { get; set; } = string.Empty;
    public string MaCanHo { get; set; } = string.Empty;
    public string TenCanHo { get; set; } = string.Empty;
    public decimal DienTich { get; set; }
    public int SoPhongNgu { get; set; }
    public int SoPhongTam { get; set; }
    public int LoaiCanHoId { get; set; }
    public int TinhTrangCanHoId { get; set; }
}
