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

    public DbSet<User> Users => Set<User>();
    public DbSet<Tokens> Tokens => Set<Tokens>();
    public DbSet<ToaNha> ToaNhas => Set<ToaNha>();
    public DbSet<Tang> Tangs => Set<Tang>();
    public DbSet<CanHo> CanHos => Set<CanHo>();
    public DbSet<QuanHeCuTru> QuanHeCuTrus => Set<QuanHeCuTru>();
    public DbSet<PhuongTien> PhuongTiens => Set<PhuongTien>();
    public DbSet<ThePhuongTien> ThePhuongTiens => Set<ThePhuongTien>();
    public DbSet<ChiSoTieuThu> ChiSoTieuThus => Set<ChiSoTieuThu>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Ignore<BaseEvent>();

        // 1. Quét và apply mọi IEntityTypeConfiguration<T> trong Assembly một cách tự động
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 2. Tự động áp dụng Global Query Filter cho Multi-tenant & Soft Delete
        SetGlobalQueryFilters(builder);
    }

    private void SetGlobalQueryFilters(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            // Kiểm tra kế thừa AuditableEntity (để áp dụng SoftDelete)
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
