namespace TGProV4.Infrastructure.Models.Identity;

public class AppRole : IdentityRole, IAuditableEntity<string>
{
    public AppRole() : base()
    {
        RoleClaims = new HashSet<AppRoleClaim>();
    }

    public AppRole(string roleName, string? roleDescription = null) : base(roleName)
    {
        Description = roleDescription;
        RoleClaims = new HashSet<AppRoleClaim>();
    }
    
    public string? Description { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string LastModifiedBy { get; set; } = default!;
    public DateTimeOffset? LastModifiedAt { get; set; }

    public ICollection<AppRoleClaim> RoleClaims { get; set; }
}
