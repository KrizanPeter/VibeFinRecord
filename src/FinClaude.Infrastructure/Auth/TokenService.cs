using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FinClaude.Application.Common.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FinClaude.Infrastructure.Auth;

public class TokenService(UserManager<IdentityUser> userManager, IOptions<JwtSettings> options) : ITokenService
{
    private const string LoginProvider = "FinClaude";
    private const string RefreshTokenName = "RefreshToken";

    private readonly JwtSettings _settings = options.Value;

    public string GenerateAccessToken(string identityUserId, Guid accountId, string email)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, identityUserId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("account_id", accountId.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    public async Task StoreRefreshTokenAsync(string userId, string refreshToken)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException($"User {userId} not found.");

        await userManager.SetAuthenticationTokenAsync(user, LoginProvider, RefreshTokenName, Hash(refreshToken));
    }

    public async Task<bool> ValidateAndRotateRefreshTokenAsync(string userId, string currentRefreshToken, string newRefreshToken)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return false;

        var stored = await userManager.GetAuthenticationTokenAsync(user, LoginProvider, RefreshTokenName);
        if (stored is null || stored != Hash(currentRefreshToken)) return false;

        await userManager.SetAuthenticationTokenAsync(user, LoginProvider, RefreshTokenName, Hash(newRefreshToken));
        return true;
    }

    private static string Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToBase64String(bytes);
    }
}
