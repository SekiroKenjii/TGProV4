namespace TGProV4.Infrastructure.Models.Identity;

public class AppUserToken : IdentityUserToken<string>, IEntity<string>
{
    public string Id { get; set; } = String.Empty;
    public DateTimeOffset Expires { get; }
    public bool IsExpired => DateTimeOffset.Now >= Expires;
    public DateTimeOffset? Revoked { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;
    
    public virtual AppUser? User { get; set; }
}
