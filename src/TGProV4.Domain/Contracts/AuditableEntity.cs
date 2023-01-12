namespace TGProV4.Domain.Contracts;

public abstract class AuditableEntity<TId> : IAuditableEntity<TId>
{
    public TId Id { get; set; } = default!;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string LastModifiedBy { get; set; } = default!;
    public DateTimeOffset? LastModifiedAt { get; set; }
}
