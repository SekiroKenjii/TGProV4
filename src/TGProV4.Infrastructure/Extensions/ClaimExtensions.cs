namespace TGProV4.Infrastructure.Extensions;

public static class ClaimExtensions
{
    public static async Task AddPermissionClaim(this ApplicationDbContext context,
                                                AppRole role,
                                                Tuple<string, string, IEnumerable<string?>> permissionDetail)
    {
        var claims = await context.RoleClaims.Where(x => x.RoleId == role.Id).ToListAsync();

        foreach (var permission in permissionDetail.Item3)
        {
            if (!claims.Any(a
                    => a.ClaimType == ApplicationConstants.ClaimTypes.Permission && a.ClaimValue == permission))
            {
                await context.RoleClaims.AddAsync(new AppRoleClaim(permissionDetail.Item2, permissionDetail.Item1) {
                    ClaimType = ApplicationConstants.ClaimTypes.Permission,
                    ClaimValue = permission
                });
            }
        }
    }
}
