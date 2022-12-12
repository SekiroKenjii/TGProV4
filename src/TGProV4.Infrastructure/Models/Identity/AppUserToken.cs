namespace TGProV4.Infrastructure.Models.Identity;

public class AppUserToken : IdentityUserToken<string>, IEntity<int>
{
    public int Id { get; set; }
    public DateTimeOffset Expires { get; } = DateTimeOffset.Now.AddDays(7);
    public bool IsExpired => DateTimeOffset.Now >= Expires;
    public DateTimeOffset? Revoked { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;

    public virtual AppUser? User { get; set; }
}
