namespace FinClaude.Application.Common.Auth;

public interface ITokenService
{
    string GenerateAccessToken(string identityUserId, Guid accountId, string email);
    string GenerateRefreshToken();
    Task StoreRefreshTokenAsync(string userId, string refreshToken);
    Task<bool> ValidateAndRotateRefreshTokenAsync(string userId, string currentRefreshToken, string newRefreshToken);
}
