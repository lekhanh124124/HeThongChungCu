namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories.ReadModels;

public class ToaNhaReadModel
{
    public int Id { get; set; }
    public string MaToaNha { get; set; } = null!;
    public string TenToaNha { get; set; } = null!;
    public int TrangThaiId { get; set; }
}
