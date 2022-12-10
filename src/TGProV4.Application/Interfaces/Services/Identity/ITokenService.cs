namespace TGProV4.Application.Interfaces.Services.Identity;

public interface ITokenService
{
    Task<TokenResponse> Login(TokenRequest request);
    Task<TokenResponse?> GetRefreshToken(RefreshTokenRequest request);
}