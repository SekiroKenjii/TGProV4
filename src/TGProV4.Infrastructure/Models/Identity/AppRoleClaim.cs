namespace TGProV4.Infrastructure.Models.Identity;

public class AppRoleClaim : IdentityRoleClaim<string>, IAuditableEntity<int>
{
    public string? Description { get; set; }
    public string? Group { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; set; }
    public string LastModifiedBy { get; set; } = default!;
    public DateTimeOffset? LastModifiedOn { get; set; }
    
    public virtual AppRole? Role { get; set; }

    public AppRoleClaim(string? roleClaimDescription = null, string? roleClaimGroup = null)
    {
        Description = roleClaimDescription;
        Group = roleClaimGroup;
    }
}