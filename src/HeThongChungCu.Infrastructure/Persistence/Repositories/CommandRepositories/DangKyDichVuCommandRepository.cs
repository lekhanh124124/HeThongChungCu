using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

internal sealed class DangKyDichVuCommandRepository : IDangKyDichVuCommandRepository
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
}
