namespace TGProV4.Infrastructure.Extensions;

public static class ClaimExtensions
{
    public static async Task AddPermissionClaim(this RoleManager<AppRole> roleManager, AppRole role, string permission)
    {
        var claims = await roleManager.GetClaimsAsync(role);

        if (!claims.Any(a => a.Type is "Permission" && a.Value == permission))
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            return;
        }

        IdentityResult.Failed();
    }
}
