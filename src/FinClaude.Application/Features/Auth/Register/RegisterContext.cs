namespace FinClaude.Application.Features.Auth.Register;

public class RegisterContext
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public string IdentityUserId { get; set; } = string.Empty;
    public Guid AccountId { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
