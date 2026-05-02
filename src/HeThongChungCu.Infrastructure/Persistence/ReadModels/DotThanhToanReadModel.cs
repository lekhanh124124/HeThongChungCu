namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

public class DotThanhToanReadModel
{
    public int TotalCount { get; set; }
    public int Id { get; set; }
    public string TenDot { get; set; } = null!;
    public int Thang { get; set; }
    public int Nam { get; set; }
    public int TrangThaiDotThanhToanId { get; set; }
    public DateTimeOffset? NgayPhatHanh { get; set; }
    public string? GhiChu { get; set; }
}
