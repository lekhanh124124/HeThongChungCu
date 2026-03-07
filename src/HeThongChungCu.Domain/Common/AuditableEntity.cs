namespace HeThongChungCu.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public int CreatedBy { get; protected set; }
    public DateTimeOffset CreatedAt { get; protected set; }
    public int? ModifiedBy { get; protected set; }
    public DateTimeOffset? ModifiedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }
    public DateTimeOffset? DeletedAt { get; protected set; }

    protected AuditableEntity()
    {
        IsDeleted = false;
    }

    protected AuditableEntity(int id) : base(id)
    {
        IsDeleted = false;
    }

    public void SetCreated(int createdBy, DateTimeOffset createdAt)
    {
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }

    public void SetModified(int modifiedBy, DateTimeOffset modifiedAt)
    {
        ModifiedBy = modifiedBy;
        ModifiedAt = modifiedAt;
    }

    public void MarkAsDeleted(DateTimeOffset deletedAt)
    {
        IsDeleted = true;
        DeletedAt = deletedAt;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}
