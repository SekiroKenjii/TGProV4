namespace TGProV4.Infrastructure.Models.Identity;

public class AppRole : IdentityRole, IAuditableEntity<string>
{
    public string? Description { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; set; }
    public string LastModifiedBy { get; set; } = default!;
    public DateTimeOffset? LastModifiedOn { get; set; }
    
    public ICollection<AppRoleClaim> RoleClaims { get; set; } = new HashSet<AppRoleClaim>();

    public AppRole(string roleName, string? roleDescription = null) : base(roleName)
    {
        Description = roleDescription;
    }
}