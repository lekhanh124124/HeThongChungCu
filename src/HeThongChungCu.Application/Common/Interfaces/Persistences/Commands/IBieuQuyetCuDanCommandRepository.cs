using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IBieuQuyetCuDanCommandRepository
{
    Task<BieuQuyetCuDan?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> HasResidentVotedAsync(int khaoSatId, int canHoId, CancellationToken cancellationToken = default);
    
    Task AddAsync(BieuQuyetCuDan bieuQuyet, CancellationToken cancellationToken = default);
    void Update(BieuQuyetCuDan bieuQuyet);
    void Delete(BieuQuyetCuDan bieuQuyet);
}
