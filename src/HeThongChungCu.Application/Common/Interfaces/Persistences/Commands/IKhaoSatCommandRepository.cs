using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IKhaoSatCommandRepository
{
    Task<KhaoSat?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<KhaoSat?> GetByIdWithQuestionsAndChoicesAsync(int id, CancellationToken cancellationToken = default);
    Task<KhaoSat?> GetByIdWithVotesAsync(int id, CancellationToken cancellationToken = default);
    Task<KhaoSat?> GetByIdWithAllAsync(int id, CancellationToken cancellationToken = default);
    Task<List<KhaoSat>> GetExpiredCampaignsAsync(DateTimeOffset now, CancellationToken cancellationToken = default);
    
    Task AddAsync(KhaoSat khaoSat, CancellationToken cancellationToken = default);
    void Update(KhaoSat khaoSat);
    void Delete(KhaoSat khaoSat);
}
