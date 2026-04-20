using ErrorOr;

namespace FinClaude.Application.Features.Auth;

public interface IIdentityService
{
    Task<ErrorOr<string>> CreateUserAsync(string email, string password);
    Task<ErrorOr<(string UserId, string Email)>> ValidateCredentialsAsync(string email, string password);
    Task<ErrorOr<(string UserId, string Email)>> FindUserByIdAsync(string userId);
}
