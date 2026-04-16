using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HeThongChungCu.Infrastructure.Persistence.Interceptors;

public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuditableEntitySaveChangesInterceptor(
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider)
    {
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var userId = _currentUserService.UserId ?? 0; // Tránh null fallback về Id system
        var now = _dateTimeProvider.Now;

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                var createdBy = entry.Entity.CreatedBy != 0 ? entry.Entity.CreatedBy : userId;
                var createdAt = entry.Entity.CreatedAt != default ? entry.Entity.CreatedAt : now;
                entry.Entity.SetCreated(createdBy, createdAt);
            }
            else if (entry.State == EntityState.Deleted)
            {
                // Soft-delete: không để EF "Modified all properties"
                entry.State = EntityState.Unchanged;

                entry.Entity.MarkAsDeleted(now);
                entry.Entity.SetModified(userId, now);

                // Chỉ update các cột cần thiết cho soft delete
                entry.Property(nameof(AuditableEntity.IsDeleted)).IsModified = true;
                entry.Property(nameof(AuditableEntity.DeletedAt)).IsModified = true;
                entry.Property(nameof(AuditableEntity.ModifiedBy)).IsModified = true;
                entry.Property(nameof(AuditableEntity.ModifiedAt)).IsModified = true;

                // Ngăn owned entries (DiaChi/Email/SoDienThoai...) bị cascade delete => NULL cột owned
                foreach (var reference in entry.References)
                {
                    var target = reference.TargetEntry;
                    if (target is not null && target.Metadata.IsOwned() && target.State == EntityState.Deleted)
                    {
                        target.State = EntityState.Unchanged;
                    }
                }

                foreach (var collection in entry.Collections)
                {
                    if (collection.CurrentValue is null) continue;
                    foreach (var dependent in collection.CurrentValue)
                    {
                        var dependentEntry = context.Entry(dependent);
                        if (dependentEntry.Metadata.IsOwned() && dependentEntry.State == EntityState.Deleted)
                        {
                            dependentEntry.State = EntityState.Unchanged;
                        }
                    }
                }
            }
            else if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.SetModified(userId, now);
            }
        }
    }
}

public static class InterceptorExtensions
{
    public static bool HasChangedOwnedEntities(this Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
