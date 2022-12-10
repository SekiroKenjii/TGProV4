namespace TGProV4.Infrastructure.Services.Identity;

public class IdentityService : ITokenService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly AppConfiguration _appConfig;

    public IdentityService(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IOptions<AppConfiguration> appConfig)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _appConfig = appConfig.Value;
    }

    public async Task<TokenResponse> Login(TokenRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            throw new UnauthorizedException(ApplicationConstants.Messages.InvalidCredentialInfo);

        if (!user.IsActive)
            throw new UnauthorizedException(ApplicationConstants.Messages.LockedUser);

        if (!user.EmailConfirmed)
            throw new UnauthorizedException(ApplicationConstants.Messages.EmailUnconfirmed);

        var checkPassword = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!checkPassword)
            throw new UnauthorizedException(ApplicationConstants.Messages.InvalidCredentialInfo);

        var refreshToken = GenerateRefreshToken();

        user.UserTokens.Add(new AppUserToken
        {
            LoginProvider = "TGProV4.Identity",
            Name = user.Email,
            Value = refreshToken
        });

        await _userManager.UpdateAsync(user);

        var token = await GenerateJwtToken(user);

        return new TokenResponse
        {
            Token = token,
            RefreshToken = refreshToken
        };
    }

    public async Task<TokenResponse?> GetRefreshToken(RefreshTokenRequest request)
    {
        var userPrincipal = GetPrincipalFromExpiredToken(request.Token!);
        var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.Users
            .Include(x => x.UserTokens)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null) return null;

        var oldToken = user.UserTokens.SingleOrDefault(x => x.Value == request.RefreshToken);

        if (oldToken is { IsActive: false })
            throw new SecurityTokenException(ApplicationConstants.Messages.TokenRevoked);

        var refreshToken = GenerateRefreshToken();

        user.UserTokens.Add(new AppUserToken
        {
            LoginProvider = "TGProV4.Identity",
            Name = user.Email,
            Value = refreshToken
        });

        await _userManager.UpdateAsync(user);

        var token = await GenerateJwtToken(user);

        return new TokenResponse
        {
            Token = token,
            RefreshToken = refreshToken
        };
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateJwtToken(AppUser user)
    {
        var credentials = GetSigningCredentials();
        var claims = await GetClaims(user);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(2),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_appConfig.Secret!);
        var key = new SymmetricSecurityKey(secret);
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    private async Task<IEnumerable<Claim>> GetClaims(AppUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();

        foreach (var role in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role));
            var roleEntity = await _roleManager.FindByNameAsync(role);
            var permissions = await _roleManager.GetClaimsAsync(roleEntity);
            permissionClaims.AddRange(permissions);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
        }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);

        return claims;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var secret = Encoding.UTF8.GetBytes(_appConfig.Secret!);
        var key = new SymmetricSecurityKey(secret);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase
            )
        ) throw new SecurityTokenException(ApplicationConstants.Messages.InvalidToken);

        return principal;
    }
}