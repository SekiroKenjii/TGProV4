namespace TGProV4.Infrastructure.Models.Identity;

public class AppUserToken : IdentityUserToken<string>, IEntity<string>
{
    public string Id { get; set; } = string.Empty;
    public DateTimeOffset Expires { get; } = DateTimeOffset.Now.AddDays(7);

    public bool IsExpired {
        get => DateTimeOffset.Now >= Expires;
    }

    public DateTimeOffset? Revoked { get; set; }

    public bool IsActive {
        get => Revoked == null && !IsExpired;
    }

    public virtual AppUser? User { get; set; }
}
