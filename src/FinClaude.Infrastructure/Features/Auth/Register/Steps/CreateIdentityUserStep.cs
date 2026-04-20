using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Auth;
using FinClaude.Application.Features.Auth.Register;

namespace FinClaude.Infrastructure.Features.Auth.Register.Steps;

public class CreateIdentityUserStep(IIdentityService identityService) : BaseStep<RegisterContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(RegisterContext context, CancellationToken ct = default)
    {
        var result = await identityService.CreateUserAsync(context.Email, context.Password);
        if (result.IsError)
            return result.Errors;

        context.IdentityUserId = result.Value;
        return await NextAsync(context, ct);
    }
}
