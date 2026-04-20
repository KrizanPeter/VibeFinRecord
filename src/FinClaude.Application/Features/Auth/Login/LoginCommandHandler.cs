using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Auth.DTOs;

namespace FinClaude.Application.Features.Auth.Login;

public class LoginCommandHandler(
    IChainProvider<LoginContext> chainProvider) : ICommandHandler<LoginCommand, AuthResponse>
{
    public async Task<ErrorOr<AuthResponse>> HandleAsync(LoginCommand command, CancellationToken ct = default)
    {
        var context = new LoginContext { Email = command.Email, Password = command.Password };
        var chain = chainProvider.GetChain();

        var result = await chain.ExecuteAsync(context, ct);
        if (result.IsError)
            return result.Errors;

        return new AuthResponse(context.AccessToken, context.RefreshToken, context.AccountId);
    }
}
