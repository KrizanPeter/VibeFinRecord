namespace FinClaude.Application.Features.Auth.Refresh;

public class RefreshContext
{
    public required string UserId { get; init; }
    public required string CurrentRefreshToken { get; init; }
    public string UserEmail { get; set; } = string.Empty;
    public Guid AccountId { get; set; }
    public string NewRefreshToken { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
}
