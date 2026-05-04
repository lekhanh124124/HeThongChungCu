namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;

public class ChiSoBatchResultResponse
{
    public int TotalItems { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<ChiSoBatchErrorDetail> Errors { get; set; } = [];
}

public class ChiSoBatchErrorDetail
{
    public int CanHoId { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
