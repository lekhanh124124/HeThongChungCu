using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class DichVuCommandRepository : IDichVuCommandRepository
{
    private readonly AppDbContext _dbContext;

    public DichVuCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DichVu?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DichVus
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<List<DichVu>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DichVus
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<DichVu?> GetByIdWithKhungGiosAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DichVus
            .Include(x => x.KhungGios)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<DichVu?> GetByIdWithBangGiasAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DichVus
            .Include(x => x.BangGias)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<DichVu?> GetByIdWithAllAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DichVus
            .Include(x => x.KhungGios)
            .Include(x => x.BangGias)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<List<DichVu>> GetByIdsWithAllAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DichVus
            .Include(x => x.KhungGios)
            .Include(x => x.BangGias)
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<BangGia?> GetBangGiaByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.BangGias
            .Include(x => (x as BangGiaLoaiCanHo)!.ChiTietGias)
            .Include(x => (x as BangGiaLuyTien)!.ChiTietGias)
            .Include(x => (x as BangGiaKhungGio)!.ChiTietGias)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<KhungGioDichVu?> GetKhungGioByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.KhungGioDichVus
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> MaDichVuExistsAsync(string maDichVu, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DichVus.AnyAsync(x => x.MaDichVu == maDichVu, cancellationToken);
    }

    public async Task<List<DichVu>> GetByHopDongAsync(int hopDongId, CancellationToken cancellationToken = default)
    {
        return await (from d in _dbContext.DichVus
                      join h in _dbContext.HopDongDoiTacs on d.Id equals h.DichVuId
                      where h.Id == hopDongId
                      select d).ToListAsync(cancellationToken);

    }

    public async Task<List<DichVu>> GetActiveMandatoryServicesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.DichVus
            .Where(x => x.IsBatBuoc && x.TrangThaiId == TrangThaiDichVu.HoatDong)
            .Include(x => x.BangGias.Where(bg => bg.IsActive))
            .Include(x => x.BangGias.Where(bg => bg.IsActive))
                .ThenInclude((BangGia bg) => ((BangGiaLoaiCanHo)bg).ChiTietGias)
            .Include(x => x.BangGias.Where(bg => bg.IsActive))
                .ThenInclude((BangGia bg) => ((BangGiaLuyTien)bg).ChiTietGias)
            .Include(x => x.BangGias.Where(bg => bg.IsActive))
                .ThenInclude((BangGia bg) => ((BangGiaKhungGio)bg).ChiTietGias)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DichVu>> GetActivePeriodicServicesWithPriceListsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.DichVus
            .Where(x => x.TrangThaiId == TrangThaiDichVu.HoatDong)
            .Include(x => x.BangGias.Where(bg => bg.IsActive && bg.IsDinhKy))
            .Include(x => x.BangGias.Where(bg => bg.IsActive && bg.IsDinhKy))
                .ThenInclude((BangGia bg) => ((BangGiaLoaiCanHo)bg).ChiTietGias)
            .Include(x => x.BangGias.Where(bg => bg.IsActive && bg.IsDinhKy))
                .ThenInclude((BangGia bg) => ((BangGiaLuyTien)bg).ChiTietGias)
            .Include(x => x.BangGias.Where(bg => bg.IsActive && bg.IsDinhKy))
                .ThenInclude((BangGia bg) => ((BangGiaKhungGio)bg).ChiTietGias)
            .Where(x => x.BangGias.Any(bg => bg.IsActive && bg.IsDinhKy))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(DichVu dichVu, CancellationToken cancellationToken = default)
    {
        await _dbContext.DichVus.AddAsync(dichVu, cancellationToken);
    }

    public void Update(DichVu dichVu)
    {
        _dbContext.DichVus.Update(dichVu);
    }

    public void Remove(DichVu dichVu)
    {
        _dbContext.DichVus.Remove(dichVu);
    }

    public void RemoveBangGia(BangGia bangGia)
    {
        _dbContext.BangGias.Remove(bangGia);
    }

    public void RemoveKhungGio(KhungGioDichVu khungGio)
    {
        _dbContext.KhungGioDichVus.Remove(khungGio);
    }
}
