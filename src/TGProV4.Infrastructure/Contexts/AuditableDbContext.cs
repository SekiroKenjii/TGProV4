namespace TGProV4.Infrastructure.Contexts;

public class AuditableDbContext
    : IdentityDbContext<AppUser, AppRole, string,
    IdentityUserClaim<string>, IdentityUserRole<string>,
    IdentityUserLogin<string>, AppRoleClaim, IdentityUserToken<string>>
{
    protected AuditableDbContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<Audit>? AuditTrails { get; set; }

    protected async Task<int> SaveChangesAsync(string? userId = null, CancellationToken cancellationToken = new())
    {
        var auditEntries = OnBeforeSaveChanges(userId);
        var result = await base.SaveChangesAsync(cancellationToken);
        await OnAfterSaveChanges(auditEntries, cancellationToken);
        return result;
    }
    
    private List<AuditEntry> OnBeforeSaveChanges(string? userId)
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();
        
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Audit || entry.State is EntityState.Detached or EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                TableName = entry.Entity.GetType().Name,
                UserId = userId
            };
            
            auditEntries.Add(auditEntry);
            
            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }

                var propertyName = property.Metadata.Name;
                
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue!;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = AuditType.Create;
                        auditEntry.NewValues[propertyName] = property.CurrentValue!;
                        break;

                    case EntityState.Deleted:
                        auditEntry.AuditType = AuditType.Delete;
                        auditEntry.OldValues[propertyName] = property.OriginalValue!;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified &&
                            property.OriginalValue?.Equals(property.CurrentValue) == false)
                        {
                            auditEntry.ChangedColumns.Add(propertyName);
                            auditEntry.AuditType = AuditType.Update;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue!;
                        }
                        break;
                    
                    case EntityState.Detached:
                    case EntityState.Unchanged:
                    default:
                        break;
                }
            }
        }
        
        foreach (var auditEntry in auditEntries.Where(x => !x.HasTemporaryProperties))
        {
            AuditTrails!.Add(auditEntry.ToAudit());
        }
        
        return auditEntries.Where(x => x.HasTemporaryProperties).ToList();
    }
    
    private Task OnAfterSaveChanges(List<AuditEntry>? auditEntries, CancellationToken cancellationToken = new())
    {
        if (auditEntries == null || auditEntries.Count == 0)
            return Task.CompletedTask;

        foreach (var auditEntry in auditEntries)
        {
            foreach (var prop in auditEntry.TemporaryProperties)
            {
                if (prop.Metadata.IsPrimaryKey())
                    auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue!;
                else
                    auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue!;
            }
            
            AuditTrails!.Add(auditEntry.ToAudit());
        }
        
        return SaveChangesAsync(cancellationToken);
    }
}