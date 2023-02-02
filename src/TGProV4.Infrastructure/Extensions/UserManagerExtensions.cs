namespace TGProV4.Infrastructure.Extensions;

public static class UserManagerExtensions
{
    public static async Task<AppUser> ValidateUserWithPassword(this AppUser? user,
                                                      UserManager<AppUser> userManager,
                                                      LoginRequest request)
    {
        user = ValidateUser(user);

        var checkPassword = request.Password is not null &&
                            await userManager.CheckPasswordAsync(user, request.Password);

        if (!checkPassword)
        {
            throw new UnauthorizedException(ApplicationConstants.Messages.InvalidCredentialInfo);
        }

        return user;
    }

    public static AppUser ValidateUser(this AppUser? user)
    {
        if (user is null)
        {
            throw new UnauthorizedException(ApplicationConstants.Messages.InvalidCredentialInfo);
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException(ApplicationConstants.Messages.LockedUser);
        }

        if (!user.EmailConfirmed)
        {
            throw new UnauthorizedException(ApplicationConstants.Messages.EmailUnconfirmed);
        }

        return user;
    }
}
