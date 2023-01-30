namespace TGProV4.Application.Interfaces.Services.Identity;

public interface ITokenService
{
    Task<TokenResponse?> Login(LoginRequest request);
    Task<TokenResponse?> GetRefreshToken(RefreshTokenRequest request);
}
