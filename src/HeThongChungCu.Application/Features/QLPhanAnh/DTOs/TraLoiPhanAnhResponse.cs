using System;

namespace HeThongChungCu.Application.Features.QLPhanAnh.DTOs;

public class TraLoiPhanAnhResponse
{
    public int Id { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public bool IsNhanVien { get; set; }
    public int CreatedBy { get; set; }
    public string TenNguoiGui { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
