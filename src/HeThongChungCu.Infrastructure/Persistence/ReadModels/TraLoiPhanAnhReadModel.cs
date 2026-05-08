using System;

namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

public class TraLoiPhanAnhReadModel
{
    public int Id { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public bool IsNhanVien { get; set; }
    public int CreatedBy { get; set; }
    public string TenNguoiGui { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
