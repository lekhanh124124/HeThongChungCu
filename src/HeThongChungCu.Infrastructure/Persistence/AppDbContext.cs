using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Infrastructure.Persistence.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using System.Reflection;

namespace HeThongChungCu.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
    private readonly IPublisher _publisher;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor,
        IPublisher publisher)
        : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
        _publisher = publisher;
    }

    public DbSet<NguoiDung> NguoiDung => Set<NguoiDung>();
    public DbSet<TaiKhoan> TaiKhoan => Set<TaiKhoan>();
    public DbSet<PhanQuyen> PhanQuyens => Set<PhanQuyen>();
    public DbSet<Tokens> Tokens => Set<Tokens>();
    public DbSet<ToaNha> ToaNhas => Set<ToaNha>();
    public DbSet<Tang> Tangs => Set<Tang>();
    public DbSet<CanHo> CanHos => Set<CanHo>();
    public DbSet<QuanHeCuTru> QuanHeCuTrus => Set<QuanHeCuTru>();
    public DbSet<PhuongTien> PhuongTiens => Set<PhuongTien>();
    public DbSet<ThePhuongTien> ThePhuongTiens => Set<ThePhuongTien>();
    public DbSet<DichVu> DichVus => Set<DichVu>();
    public DbSet<BangGia> BangGias => Set<BangGia>();
    public DbSet<BangGiaLuyTien> BangGiaLuyTiens => Set<BangGiaLuyTien>();
    public DbSet<HoaDon> HoaDons => Set<HoaDon>();
    public DbSet<ChiTietHoaDon> ChiTietHoaDons => Set<ChiTietHoaDon>();
    public DbSet<ThanhToan> ThanhToans => Set<ThanhToan>();
    public DbSet<LaiChamTra> LaiChamTras => Set<LaiChamTra>();
    public DbSet<CauHinhLai> CauHinhLais => Set<CauHinhLai>();
    public DbSet<ChiSoTieuThu> ChiSoTieuThus => Set<ChiSoTieuThu>();
    public DbSet<DangKyDichVu> DangKyDichVus => Set<DangKyDichVu>();
    public DbSet<TaiLieuNguoiDung> TaiLieuNguoiDungs => Set<TaiLieuNguoiDung>();
    public DbSet<TepTaiLieu> TepTaiLieus => Set<TepTaiLieu>();
    public DbSet<YeuCauCuTru> YeuCauCuTrus => Set<YeuCauCuTru>();
    public DbSet<YeuCauTaiLieuCuTru> YeuCauTaiLieuCuTrus => Set<YeuCauTaiLieuCuTru>();
    public DbSet<YeuCauPhuongTien> YeuCauPhuongTiens => Set<YeuCauPhuongTien>();
    public DbSet<ThongBao> ThongBaos => Set<ThongBao>();
    public DbSet<PhanBoThongBao> PhanBoThongBaos => Set<PhanBoThongBao>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Ignore<BaseEvent>();
        
        // Tự động Ignore tất cả các class kế thừa từ BaseEnum trong Domain assembly
        var smartEnumTypes = typeof(BaseEnum<,>).Assembly.GetTypes()
            .Where(t => t.BaseType != null && 
                        t.BaseType.IsGenericType &&
                        t.BaseType.GetGenericTypeDefinition() == typeof(BaseEnum<,>));

        foreach (var type in smartEnumTypes)
        {
            builder.Ignore(type);
        }

        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(builder);

        // 2. Tự động áp dụng Global Query Filter cho Multi-tenant & Soft Delete
        SetGlobalQueryFilters(builder);
    }

    private void SetGlobalQueryFilters(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            // Kiểm tra kế thừa AuditableEntity (để áp dụng SoftDelete)
            // LƯU Ý: Chỉ áp dụng filter cho Root Entity trong mô hình thừa kế (TPH)
            if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(nameof(ApplySoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.MakeGenericMethod(entityType.ClrType);

                method?.Invoke(this, [builder]);
            }
        }
    }

    private void ApplySoftDeleteFilter<T>(ModelBuilder builder) where T : AuditableEntity
    {
        builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Đăng ký Interceptor vô DbContext
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchDomainEventsAsync();

        return result;
    }

    private async Task DispatchDomainEventsAsync()
    {
        var domainEntities = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Database.CurrentTransaction == null)
        {
            await Database.BeginTransactionAsync(cancellationToken);
        }
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Database.CurrentTransaction != null)
        {
            await Database.CurrentTransaction.CommitAsync(cancellationToken);
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Database.CurrentTransaction != null)
        {
            await Database.CurrentTransaction.RollbackAsync(cancellationToken);
        }
    }

    public DbConnection GetDbConnection()
    {
        return Database.GetDbConnection();
    }

    public DbTransaction? GetDbTransaction()
    {
        return Database.CurrentTransaction?.GetDbTransaction();
    }

    public async Task<TResponse> ExecuteAsync<TResponse>(Func<Task<TResponse>> action)
    {
        var strategy = Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(action);
    }
}
