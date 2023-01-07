namespace TGProV4.Infrastructure.Models.Identity;

public class AppUserToken : IEntity<int>
{
    public AppUserToken(string token)
    {
        Token = token;
        Expires = DateTimeOffset.Now.AddDays(7);
    }
    
    public int Id { get; set; }
    public string? Token { get; }
    public DateTimeOffset Expires { get; }
    public bool IsExpired => DateTimeOffset.Now >= Expires;
    public DateTimeOffset? Revoked { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;

    public string? UserId { get; set; }
    public virtual AppUser? User { get; set; }
}
