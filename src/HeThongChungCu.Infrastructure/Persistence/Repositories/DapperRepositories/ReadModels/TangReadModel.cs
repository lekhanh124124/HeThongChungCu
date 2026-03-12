namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories.ReadModels;

public class TangReadModel
{
    public int Id { get; set; }
    public int ToaNhaId { get; set; }
    public string MaTang { get; set; } = null!;
    public string TenTang { get; set; } = null!;
}
