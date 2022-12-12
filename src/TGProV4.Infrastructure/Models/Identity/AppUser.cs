namespace TGProV4.Infrastructure.Models.Identity;

public class AppUser : IdentityUser<string>, IAuditableEntity<string>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; set; }
    public string LastModifiedBy { get; set; } = default!;
    public DateTimeOffset? LastModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public bool IsActive { get; set; }

    public ICollection<AppUserToken> UserTokens { get; set; } = new HashSet<AppUserToken>();
}
