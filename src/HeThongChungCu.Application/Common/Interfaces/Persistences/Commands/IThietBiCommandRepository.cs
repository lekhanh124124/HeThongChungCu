using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IThietBiCommandRepository
{
    Task<ThietBi?> GetThietBiByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> MaThietBiExistsAsync(string maThietBi, CancellationToken cancellationToken = default);
    Task AddThietBiAsync(ThietBi thietBi, CancellationToken cancellationToken = default);
    void UpdateThietBi(ThietBi thietBi);

    Task<HangMucBaoTri?> GetHangMucByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> MaHangMucExistsAsync(string maHangMuc, CancellationToken cancellationToken = default);
    Task AddHangMucAsync(HangMucBaoTri hangMuc, CancellationToken cancellationToken = default);
    void UpdateHangMuc(HangMucBaoTri hangMuc);

    Task<LichBaoTri?> GetLichBaoTriByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<LichBaoTri>> GetActiveLichBaoTrisAsync(DateTimeOffset date, CancellationToken cancellationToken = default);
    Task AddLichBaoTriAsync(LichBaoTri lichBaoTri, CancellationToken cancellationToken = default);
    void UpdateLichBaoTri(LichBaoTri lichBaoTri);
}
