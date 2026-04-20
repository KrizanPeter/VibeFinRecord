namespace FinClaude.Application.Features.Auth.DTOs;

public record AuthResponse(string AccessToken, string RefreshToken, Guid AccountId);

public record RefreshResponse(string AccessToken, string RefreshToken);
