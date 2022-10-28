using Microsoft.AspNetCore.Identity;
using TGProV4.Domain.Contracts;

namespace TGProV4.Infrastructure.Models.Identity;

public class AppUser : IdentityUser<string>, IAuditableEntity<string>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; set; }
    public string LastModifiedBy { get; set; } = default!;
    public DateTimeOffset? LastModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public bool IsActive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpiryTime { get; set; }
}