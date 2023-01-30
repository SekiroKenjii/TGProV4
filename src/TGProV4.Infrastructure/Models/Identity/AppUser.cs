namespace TGProV4.Infrastructure.Models.Identity;

public class AppUser : IdentityUser<string>, IAuditableEntity<string>
{
    public AppUser() => UserTokens = new HashSet<AppUserToken>();

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? AvatarId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string LastModifiedBy { get; set; } = default!;
    public DateTimeOffset? LastModifiedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public bool IsActive { get; set; }
    public Gender Gender { get; set; }

    public ICollection<AppUserToken> UserTokens { get; set; }
}
