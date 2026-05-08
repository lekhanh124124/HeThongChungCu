namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

public class CauHoiKhaoSatReadModel
{
    public int Id { get; set; }
    public int KhaoSatId { get; set; }
    public string NoiDungCauHoi { get; set; } = string.Empty;
    public bool IsBatBuoc { get; set; }
    public bool IsMultiSelect { get; set; }
}
