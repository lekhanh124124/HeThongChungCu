namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories.ReadModels;

public class CanHoReadModel
{
    public int Id { get; set; }
    public int TangId { get; set; }
    public string MaCanHo { get; set; } = null!;
    public int TinhTrangCanHoId { get; set; }
}
