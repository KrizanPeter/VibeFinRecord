using ErrorOr;
using FinClaude.Application.Features.Auth;
using Microsoft.AspNetCore.Identity;

namespace FinClaude.Infrastructure.Features.Auth;

public class IdentityService(UserManager<IdentityUser> userManager) : IIdentityService
{
    public async Task<ErrorOr<string>> CreateUserAsync(string email, string password)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
            return Error.Conflict("Auth.EmailTaken", "An account with this email already exists.");

        var user = new IdentityUser { UserName = email, Email = email };
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var desc = string.Join("; ", result.Errors.Select(e => e.Description));
            return Error.Validation("Auth.InvalidCredentials", desc);
        }

        return user.Id;
    }

    public async Task<ErrorOr<(string UserId, string Email)>> ValidateCredentialsAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null || user.Email is null)
            return Error.Validation("Auth.InvalidCredentials", "Invalid email or password.");

        var valid = await userManager.CheckPasswordAsync(user, password);
        if (!valid)
            return Error.Validation("Auth.InvalidCredentials", "Invalid email or password.");

        return (user.Id, user.Email);
    }

    public async Task<ErrorOr<(string UserId, string Email)>> FindUserByIdAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null || user.Email is null)
            return Error.NotFound("Auth.UserNotFound", "User not found.");

        return (user.Id, user.Email);
    }
}
