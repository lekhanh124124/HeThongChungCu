using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public sealed class DangKyDichVuCommandRepository : IDangKyDichVuCommandRepository
{
    private readonly AppDbContext _context;

    public DangKyDichVuCommandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(DangKyDichVu dangKyDichVu, CancellationToken cancellationToken)
    {
        await _context.DangKyDichVus.AddAsync(dangKyDichVu, cancellationToken);
    }

    public async Task<int> GetSumActiveQuantityByDichVuIdAsync(int dichVuId, CancellationToken cancellationToken)
    {
        return await _context.DangKyDichVus
            .Where(x => x.DichVuId == dichVuId && x.TrangThaiDangKyId == TrangThaiDangKy.DangSuDung)
            .SumAsync(x => x.SoLuong, cancellationToken);
    }

    public async Task<int> GetSumActiveQuantityByKhungGioAsync(int dichVuId, TimeSpan gioBatDau, TimeSpan gioKetThuc, DateTime ngay, CancellationToken cancellationToken)
    {
        var targetDate = ngay.Date;
        return await _context.DangKyDichVus
            .Where(x => x.DichVuId == dichVuId
                     && x.ThoiGian.NgayBatDau.DateTime.TimeOfDay == gioBatDau
                     && x.ThoiGian.NgayKetThuc != null
                     && x.ThoiGian.NgayKetThuc.Value.DateTime.TimeOfDay == gioKetThuc
                     && x.TrangThaiDangKyId == TrangThaiDangKy.DangSuDung
                     && x.ThoiGian.NgayBatDau.Date == targetDate)
            .SumAsync(x => x.SoLuong, cancellationToken);
    }

    public async Task<bool> IsCanHoRegisteredActiveAsync(int canHoId, int dichVuId, CancellationToken cancellationToken)
    {
        return await _context.DangKyDichVus
            .AnyAsync(x => x.CanHoId == canHoId
                        && x.DichVuId == dichVuId
                        && x.TrangThaiDangKyId == TrangThaiDangKy.DangSuDung, cancellationToken);
    }

    public async Task<List<DangKyDichVu>> GetActiveSubscriptionsByCanHoAsync(int canHoId, CancellationToken cancellationToken = default)
    {
        return await _context.DangKyDichVus
            .Where(x => x.CanHoId == canHoId && x.TrangThaiDangKyId == TrangThaiDangKy.DangSuDung)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DangKyDichVu>> GetActiveByCanHoIdsAsync(IEnumerable<int> canHoIds, CancellationToken cancellationToken = default)
    {
        return await _context.DangKyDichVus
            .Where(x => canHoIds.Contains(x.CanHoId) && x.TrangThaiDangKyId == TrangThaiDangKy.DangSuDung)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DangKyDichVu>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DangKyDichVus
            .Where(x => x.TrangThaiDangKyId == TrangThaiDangKy.DangSuDung)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyByDichVuIdAsync(int dichVuId, CancellationToken cancellationToken = default)
    {
        return await _context.DangKyDichVus.AnyAsync(x => x.DichVuId == dichVuId, cancellationToken);
    }

    public void Update(DangKyDichVu dangKyDichVu)
    {
        _context.DangKyDichVus.Update(dangKyDichVu);
    }
}
