namespace HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

public class HangMucBaoTriResponse
{
    public int Id { get; set; }
    public string MaHangMuc { get; set; } = null!;
    public string TenHangMuc { get; set; } = null!;
    public string? MoTa { get; set; }
    public int ThoiGianUocTinhPhut { get; set; }
    public decimal ChiPhiUocTinh { get; set; }
    public List<string> ChecklistTieuChuan { get; set; } = new();
}

public class HangMucBaoTriDetailResponse : HangMucBaoTriResponse
{
    // Kế thừa toàn bộ trường thông tin chi tiết từ HangMucBaoTriResponse
}
